using System;
using UnityEngine;
using Task = System.Threading.Tasks.Task;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public event EventHandler OnRewardDecidedEvent;
    public event EventHandler OnSuccessfulSpinEvent;
    public event EventHandler OnSpinCompleteEvent;
    public event EventHandler OnBronzeSpinEvent;
    public event EventHandler OnSilverSpinReachedEvent;
    public event EventHandler OnSuperSpinReachedEvent;
    private bool isBoom;
    private int _successfulSpinCounter;
    public int SuccessfulSpinCounter => _successfulSpinCounter;
    [SerializeField] private GameState _state;
    public GameState State => _state;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Game Manager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _successfulSpinCounter = 0;
        AddEvent();
    }

    private void OnDisable()
    {
        RemoveEvent();
    }


    public void UpdateGameState(GameState newState)
    {
        _state = newState;
        switch (newState)
        {
            case GameState.Start:
                HandleStart();
                break;
            case GameState.Spin:
                HandleSpin();
                break;
            case GameState.Decision:
                HandleDecision();
                break;
            case GameState.Reward:
                HandleReward();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void HandleStart()
    {
        OnSpinCompleteEvent?.Invoke(this, EventArgs.Empty);
        HandleSpecialSpins();
    }
    
    private async void  HandleSpin()
    {
        await Task.Delay(3000);
        UpdateGameState(GameState.Decision);
    }
    
    private async void HandleDecision()
    {
        await Task.Delay(1000);
        if (isBoom)
        {
            UpdateGameState(GameState.GameOver);
        }

        if (!isBoom)
        {
            UpdateGameState(GameState.Reward);
        }
    }
    
    private async void HandleGameOver()
    {
        _successfulSpinCounter = 0;
        OnRewardDecidedEvent?.Invoke(this, EventArgs.Empty);
        await Task.Delay(1000);
        UpdateGameState(GameState.Start);
    }

    private async void HandleReward()
    {
        _successfulSpinCounter++;
        OnRewardDecidedEvent?.Invoke(this, EventArgs.Empty);
        OnSuccessfulSpinEvent?.Invoke(this, EventArgs.Empty);
        await Task.Delay(1000);
        UpdateGameState(GameState.Start);
    }

    private void HandleSpecialSpins()
    {
        if (SuccessfulSpinCounter % 5 == 0 && SuccessfulSpinCounter % 30 != 0 && SuccessfulSpinCounter !=0)
        {
            OnSilverSpinReachedEvent?.Invoke(this, EventArgs.Empty);
        }
        if (SuccessfulSpinCounter % 30 == 0 && SuccessfulSpinCounter !=0)
        {
            OnSuperSpinReachedEvent?.Invoke(this, EventArgs.Empty);
        }
        if((SuccessfulSpinCounter % 5 != 0 && SuccessfulSpinCounter % 30 != 0))
        {
            OnBronzeSpinEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AddEvent()
    {
        RewardManager.Instance.OnRewardSelectedEvent += OnRewardSelected;
    }

    private void RemoveEvent()
    {
        RewardManager.Instance.OnRewardSelectedEvent -= OnRewardSelected;
    }

    private void OnRewardSelected(object sender, bool e)
    {
        isBoom = e;
    }
}


    

/*stateler

1- Start
2- Spin
3- RewardView e??er bomba gelmezse start'a geri d??n??yor e??er bomba gelirse gameover'a
4- GameOver - buttona bas??nca starta d??ner.
5- Silver Spin -5 spinde 1 
6- Gold Spin -30 spinde 1

*/