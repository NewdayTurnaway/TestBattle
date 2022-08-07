using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleScripts
{
    internal class MainWindowMediator : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private TMP_Text _countMoneyText;
        [SerializeField] private TMP_Text _countHealthText;
        [SerializeField] private TMP_Text _countPowerText;
        [SerializeField] private TMP_Text _crimeLevelText;

        [Header("Enemy Stats")]
        [SerializeField] private TMP_Text _countPowerEnemyText;

        [Header("Money Buttons")]
        [SerializeField] private Button _addMoneyButton;
        [SerializeField] private Button _minusMoneyButton;

        [Header("Health Buttons")]
        [SerializeField] private Button _addHealthButton;
        [SerializeField] private Button _minusHealthButton;

        [Header("Power Buttons")]
        [SerializeField] private Button _addPowerButton;
        [SerializeField] private Button _minusPowerButton;
        
        [Header("Crime Level Button")]
        [SerializeField] private Button _crimeLevelButton;

        [Header("Other Buttons")]
        [SerializeField] private Button _passButton;
        [SerializeField] private Button _fightButton;

        private const int CRIME_LEVEL_LIMIT = 6;

        private PlayerData _money;
        private PlayerData _heath;
        private PlayerData _power;
        private PlayerData _crimeLevel;

        private Enemy _enemy;


        private void Start()
        {
            _enemy = new Enemy("Enemy Flappy");

            _money = CreatePlayerData(DataType.Money);
            _heath = CreatePlayerData(DataType.Health);
            _power = CreatePlayerData(DataType.Power);
            _crimeLevel = CreatePlayerData(DataType.CrimeLevel);

            Subscribe();

            ChangeFightButtons(_crimeLevel);
        }

        private void OnDestroy()
        {
            DisposePlayerData(ref _money);
            DisposePlayerData(ref _heath);
            DisposePlayerData(ref _power);
            DisposePlayerData(ref _crimeLevel);

            Unsubscribe();
        }


        private PlayerData CreatePlayerData(DataType dataType)
        {
            PlayerData playerData = new(dataType);
            playerData.ValueChanged += _enemy.Update;

            return playerData;
        }

        private void DisposePlayerData(ref PlayerData playerData)
        {
            playerData.ValueChanged -= _enemy.Update;
            playerData = null;
        }


        private void Subscribe()
        {
            _addMoneyButton.onClick.AddListener(IncreaseMoney);
            _minusMoneyButton.onClick.AddListener(DecreaseMoney);

            _addHealthButton.onClick.AddListener(IncreaseHealth);
            _minusHealthButton.onClick.AddListener(DecreaseHealth);

            _addPowerButton.onClick.AddListener(IncreasePower);
            _minusPowerButton.onClick.AddListener(DecreasePower);

            _crimeLevelButton.onClick.AddListener(ChangeCrimeLevel);

            _passButton.onClick.AddListener(PassFight);
            _fightButton.onClick.AddListener(Fight);
        }

        private void Unsubscribe()
        {
            _addMoneyButton.onClick.RemoveListener(IncreaseMoney);
            _minusMoneyButton.onClick.RemoveListener(DecreaseMoney);

            _addHealthButton.onClick.RemoveListener(IncreaseHealth);
            _minusHealthButton.onClick.RemoveListener(DecreaseHealth);

            _addPowerButton.onClick.RemoveListener(IncreasePower);
            _minusPowerButton.onClick.RemoveListener(DecreasePower);

            _crimeLevelButton.onClick.RemoveListener(ChangeCrimeLevel);

            _passButton.onClick.RemoveListener(PassFight);
            _fightButton.onClick.RemoveListener(Fight);
        }


        private void IncreaseMoney() => IncreaseValue(_money);
        private void DecreaseMoney() => DecreaseValue(_money);

        private void IncreaseHealth() => IncreaseValue(_heath);
        private void DecreaseHealth() => DecreaseValue(_heath);

        private void IncreasePower() => IncreaseValue(_power);
        private void DecreasePower() => DecreaseValue(_power);
        

        private void IncreaseValue(PlayerData playerData) => AddToValue(1, playerData);
        private void DecreaseValue(PlayerData playerData) => AddToValue(-1, playerData);

        private void AddToValue(int addition, PlayerData playerData)
        {
            playerData.Value += addition;
            ChangeDataWindow(playerData);
        }

        private void ChangeCrimeLevel() => NextCrimeLevel(_crimeLevel);

        private void NextCrimeLevel(PlayerData playerData)
        {
            playerData.Value = (playerData.Value + 1) % CRIME_LEVEL_LIMIT;
            ChangeDataWindow(playerData);
            ChangeFightButtons(playerData);
        }

        private void ChangeFightButtons(PlayerData playerData)
        {
            bool canPass = playerData.Value <= 2;
            _passButton.gameObject.SetActive(canPass);
            _fightButton.gameObject.SetActive(!canPass);
        }

        private void ChangeDataWindow(PlayerData playerData)
        {
            int value = playerData.Value;
            DataType dataType = playerData.DataType;
            TMP_Text textComponent = GetTextComponent(dataType);
            textComponent.text = $"Player {dataType:F}: {value}";

            int enemyPower = _enemy.CalcPower();
            _countPowerEnemyText.text = $"Enemy Power: {enemyPower}";
        }

        private TMP_Text GetTextComponent(DataType dataType) =>
            dataType switch
            {
                DataType.Money => _countMoneyText,
                DataType.Health => _countHealthText,
                DataType.Power => _countPowerText,
                DataType.CrimeLevel => _crimeLevelText,
                _ => throw new ArgumentException($"Wrong {nameof(DataType)}")
            };


        private void Fight()
        {
            int enemyPower = _enemy.CalcPower();
            bool isVictory = _power.Value >= enemyPower;

            string color = isVictory ? "#07FF00" : "#FF0000";
            string message = isVictory ? "Win" : "Lose";

            Debug.Log($"<color={color}>{message}!!!</color>");
        }

        private void PassFight()
        {
            Debug.Log($"<color=white>Pass</color>");
        }
    }
}
