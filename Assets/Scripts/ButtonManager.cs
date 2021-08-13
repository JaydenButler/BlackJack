using System;
using System.Collections.Generic;
using System.Linq;
using Structs;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * TODO:
 * === GENREAL ===
 * - Put things into methods
 * - Fix hands not resetting on 5th card
 * === GAME BREAKING ===
 * - Check for double aces on starting hand
 * - Sometimes cards don't appear
 * - If player gets 21 in two cards, dealer cards are not dealt sometiems
 */

public class ButtonManager : MonoBehaviour
{
    
    //Buttons
    [SerializeField] private Button dealCardsButton;
    [SerializeField] private Button playerHitButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button exitButton;

    //Dealer Cards
    [SerializeField] private GameObject dealerCardOne;
    [SerializeField] private GameObject dealerCardTwo;
    [SerializeField] private GameObject dealerCardThree;
    [SerializeField] private GameObject dealerCardFour;
    [SerializeField] private GameObject dealerCardFive;
    
    //Player Cards
    [SerializeField] private GameObject playerCardOne;
    [SerializeField] private GameObject playerCardTwo;
    [SerializeField] private GameObject playerCardThree;
    [SerializeField] private GameObject playerCardFour;
    [SerializeField] private GameObject playerCardFive;

    //Text
    [SerializeField] private Text playerCardValue;
    [SerializeField] private Text dealerCardValue;
    [SerializeField] private Text endGameText;

    private enum GameState
    {
        Win,
        Loss,
        Playing,
        Idle
    }

    private GameState _gameState = GameState.Idle;
    
    private int _deckSize;
    private List<Card> _deck;
    private CurrentHands _currentHands;

    // Start is called before the first frame update
    private void Start()
    {
        dealCardsButton.onClick.AddListener(DealCards);
        playerHitButton.onClick.AddListener(PlayerHit);
        resetButton.onClick.AddListener(SetupGame);
        standButton.onClick.AddListener(PlayerStand);
        exitButton.onClick.AddListener(OnExitButtonPressed);

        SetupGame();
    }

    private void OnExitButtonPressed()
    {
        Application.Quit();
    }

    private void DealerPlay()
    {
        //Flip their 2nd card
        if (!_currentHands.DealerHand.Cards[1].DisplayCard)
        {
            _currentHands.DealerHand.Cards[1].DisplayCard = true;
            dealerCardTwo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        //Calculate the new values
        CalculateCardValues();
        //If the dealer doesnt have 21
        if (_currentHands.DealerHand.CardValues < 21)
        {
            //If the dealer has greater than or equal to 17
            if (_currentHands.DealerHand.CardValues >= 17)
            {
                //If the Dealer's Cards are higher than the Player's Cards - Player loses
                if (_currentHands.DealerHand.CardValues > _currentHands.PlayerHand.CardValues)
                {
                    _gameState = GameState.Loss;
                    EndGame();
                }
                //If the Player's Cards are higher than the Dealer's Card - Player wins
                else if (_currentHands.DealerHand.CardValues < _currentHands.PlayerHand.CardValues)
                {
                    _gameState = GameState.Win;
                    EndGame();
                }
                //If the Dealer's and Player's Cards are the same value
                else if (_currentHands.DealerHand.CardValues == _currentHands.PlayerHand.CardValues)
                {
                    //If the Dealer has less or equal to card count as the Player - Player loses
                    if (_currentHands.DealerHand.Cards.Count <= _currentHands.PlayerHand.Cards.Count)
                    {
                        _gameState = GameState.Loss;
                        EndGame();
                    }
                    //If the Player has less card count than the dealer - Player wins
                    else if (_currentHands.DealerHand.Cards.Count > _currentHands.PlayerHand.Cards.Count)
                    {
                        _gameState = GameState.Win;
                        EndGame();
                    }
                }
            }
            //If the dealer is on 16 or below
            else if (_currentHands.DealerHand.CardValues < 17)
            {
                //Deal cards
                switch (_currentHands.DealerHand.Cards.Count)
                {
                    case 2:
                        InitDealerCard(dealerCardThree.transform);
                        break;
                    case 3:
                        InitDealerCard(dealerCardFour.transform);
                        break;
                    case 4:
                        InitDealerCard(dealerCardFive.transform);
                        break;
                }
                DealerPlay();
            }
            //If the Dealer gets 21 in 2 cards - Player loses
            else if (_currentHands.DealerHand.CardValues == 21)
            {
                _gameState = GameState.Loss;
                EndGame();
            }
        }
        //If the Dealer busts - Player wins
        else if (_currentHands.DealerHand.CardValues > 21)
        {
            _gameState = GameState.Win;
            EndGame();
        }
        //If the Dealer has 21
        else if (_currentHands.DealerHand.CardValues == 21)
        {
            //If the Dealer has 21 in less than or equal to cards as the Player - Player loses
            if (_currentHands.DealerHand.Cards.Count <= _currentHands.PlayerHand.Cards.Count)
            {
                _gameState = GameState.Loss;
                EndGame();
            }
            //If the Player has 21 in less cards than the Dealer - Player wins
            else if (_currentHands.DealerHand.Cards.Count > _currentHands.PlayerHand.Cards.Count)
            {
                _gameState = GameState.Win;
                EndGame();
            }
        }
    }

    private void PlayerStand()
    {
        DealerPlay();
    }

    private void EndGame()
    {
        switch (_gameState)
        {
            case GameState.Loss:
                endGameText.text = "You lost!";
                endGameText.gameObject.SetActive(true);
                break; 
            case GameState.Win:
                endGameText.text = "You won!";
                endGameText.gameObject.SetActive(true);
                break;
            default:
                endGameText.text = "Something broke!!";
                endGameText.gameObject.SetActive(true);
                break;
        }
    }

    private void SetupGame()
    {
        if(playerCardOne.transform.childCount != 0) { Destroy(playerCardOne.transform.GetChild(0).gameObject); }
        if(playerCardTwo.transform.childCount != 0) { Destroy(playerCardTwo.transform.GetChild(0).gameObject); }
        if(playerCardThree.transform.childCount != 0) { Destroy(playerCardThree.transform.GetChild(0).gameObject); }
        if(playerCardFour.transform.childCount != 0) { Destroy(playerCardFour.transform.GetChild(0).gameObject); }
        if(playerCardFive.transform.childCount != 0) { Destroy(playerCardFive.transform.GetChild(0).gameObject); }
        
        if(dealerCardOne.transform.childCount != 0) { Destroy(dealerCardOne.transform.GetChild(0).gameObject); }
        if(dealerCardTwo.transform.childCount != 0) { Destroy(dealerCardTwo.transform.GetChild(0).gameObject); }
        if(dealerCardThree.transform.childCount != 0) { Destroy(dealerCardThree.transform.GetChild(0).gameObject); }
        if(dealerCardFour.transform.childCount != 0) { Destroy(dealerCardFour.transform.GetChild(0).gameObject); }
        if(dealerCardFive.transform.childCount != 0) { Destroy(dealerCardFive.transform.GetChild(0).gameObject); }

        _deckSize = 51;
        _deck = new List<Card>();
        _currentHands = new CurrentHands();
        PlayerHand playerHand = new PlayerHand
        {
            Cards = new List<Card>(),
            CardValues = 0
        };

        DealerHand dealerHand = new DealerHand
        {
            Cards = new List<Card>(),
            CardValues = 0
        };
        _currentHands.PlayerHand = playerHand;
        _currentHands.DealerHand = dealerHand;
        playerCardValue.text = "Your card values: 0";
        dealerCardValue.text = "Dealer card values: 0";
        
        endGameText.gameObject.SetActive(false);

        dealCardsButton.interactable = true;

        _gameState = GameState.Playing;
    }

    private void CalculateCardValues()
    {
        int i = 0;
        
        foreach (var card in _currentHands.PlayerHand.Cards)
        {
            i += card.Value;
        }

        _currentHands.PlayerHand.CardValues = i;

        i = 0;
        
        foreach (var card in _currentHands.DealerHand.Cards)
        {
            if(card.DisplayCard) { i += card.Value; }
        }

        _currentHands.DealerHand.CardValues = i;
        
        playerCardValue.text = $"Your card values: {_currentHands.PlayerHand.CardValues}";
        dealerCardValue.text = $"Your card values: {_currentHands.DealerHand.CardValues}";
    }
    bool CheckDealerBust()
    {
        return _currentHands.DealerHand.CardValues > 21;
    }
    bool CheckPlayerBust()
    {
        return _currentHands.PlayerHand.CardValues > 21;
    }

    private void CheckWin()
    {
        if (CheckPlayerBust())
        {
            if (_currentHands.PlayerHand.Cards.Any(x => x.DisplayValue == "A"))
            {
                foreach (var card in _currentHands.PlayerHand.Cards)
                {
                    if (card.DisplayValue == "A")
                    {
                        card.Value = 1;
                        CalculateCardValues();
                        if (CheckPlayerBust())
                        {
                            _gameState = GameState.Loss;
                            EndGame();
                        }
                        else if (_currentHands.PlayerHand.CardValues == 21)
                        {
                            DealerPlay();
                        }
                    }
                }

                if (_currentHands.DealerHand.Cards.Any(x => x.DisplayValue == "A"))
                {
                    foreach (var card in _currentHands.DealerHand.Cards)
                    {
                        if (card.DisplayValue == "A")
                        {
                            card.Value = 1;
                            DealerPlay();
                        }
                    }
                }
            }
            else
            {
                _gameState = GameState.Loss;
                EndGame();
            }
        }
        else if (_currentHands.PlayerHand.CardValues == 21)
        {
            DealerPlay();
        }
    }

    private void InitDealerCard(Transform parentTransform)
    {
        int prefabIndex = Random.Range(0, _deckSize);
        GameObject _ = Instantiate(_deck[prefabIndex].CardObject, parentTransform) as GameObject;
        if (_gameState == GameState.Playing && _currentHands.DealerHand.Cards.Count == 1)
        {
            _deck[prefabIndex].DisplayCard = false;
        }
        _currentHands.DealerHand.Cards.Add(_deck[prefabIndex]);
        CalculateCardValues();
        _deck.Remove(_deck[prefabIndex]);
        _deckSize -= 1;

        dealerCardValue.text = $"Your card values: {_currentHands.DealerHand.CardValues}";
        CheckWin();
    }

    private void InitPlayerCard(Transform parentTransform)
    {
        int prefabIndex = Random.Range(0, _deckSize);
        GameObject _ = Instantiate(_deck[prefabIndex].CardObject, parentTransform) as GameObject;
        _currentHands.PlayerHand.Cards.Add(_deck[prefabIndex]);
        CalculateCardValues();
        _deck.Remove(_deck[prefabIndex]);
        _deckSize -= 1;

        playerCardValue.text = $"Your card values: {_currentHands.PlayerHand.CardValues}";
        CheckWin();
    }

    private void PlayerHit()
    {
        if (_currentHands.PlayerHand.Cards.Count == 2)
        {
            InitPlayerCard(playerCardThree.transform);
        }
        else if (_currentHands.PlayerHand.Cards.Count == 3)
        {
            InitPlayerCard(playerCardFour.transform);
        }
        else if (_currentHands.PlayerHand.Cards.Count == 4)
        {
            InitPlayerCard(playerCardFive.transform);
            if (CheckPlayerBust())
            {
                _gameState = GameState.Loss;
                EndGame();
            }
            else
            {
                DealerPlay();
            }
        }
    }

    private void GenerateCards()
    {
        var cardObjects = Resources.LoadAll("Prefab/BackColor_Black").Cast<GameObject>().ToList();

        _deck = new List<Card>();

        foreach (var card in cardObjects)
        {
            int startIndex = 19;
            var newName = card.name.Substring(startIndex, card.name.Length - startIndex);
            newName = newName.Substring(0, newName.Length - 3);

            var suit = newName.Substring(0, newName.Length - 2);
            var value = Int32.Parse(newName.Substring(newName.Length - 2));

            string displayValue = $"{value}";

            #region Display Values

            if (value == 1)
            {
                displayValue = "A";
                value = 11;
            }
            else if (value == 11)
            {
                displayValue = "J";
                value = 10;
            }
            else if (value == 12)
            {
                displayValue = "Q";
                value = 10;
            }
            else if (value == 13)
            {
                displayValue = "K";
                value = 10;
            }

            #endregion

            Card newCard = new Card
            {
                Suit = suit,
                DisplayValue = displayValue,
                Value = value,
                CardObject = card.gameObject,
                DisplayCard = true
            };
            _deck.Add(newCard);
        }
    }
    
    void DealCards()
    {
        GenerateCards();

        if (dealerCardTwo.transform.rotation == Quaternion.Euler(new Vector3(0, 0, 0)))
        {
            dealerCardTwo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        
        InitPlayerCard(playerCardOne.transform);
        InitPlayerCard(playerCardTwo.transform);
        InitDealerCard(dealerCardOne.transform);
        InitDealerCard(dealerCardTwo.transform);

        dealCardsButton.interactable = false;
        
        CheckWin();
    }

    // Update is called once per frame
    void Update()
    {
    }
}