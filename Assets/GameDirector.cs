using UnityEngine;
using TMPro;

public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance { get; private set; }

    [System.Serializable]
    public class LevelConfig
    {
        public string levelName = "Level 1";
        [Tooltip("本关倒计时（秒）")]
        public float duration = 20f;
        [Tooltip("本关过关目标分数（设为 0 则时间到即过关）")]
        public int targetScore = 300;

        [Header("难度参数（可在 Inspector 自由调整）")]
        [Tooltip("物件产生间隔（秒），数值越小产生越快")]
        public float spawnInterval = 1.0f;
        [Tooltip("物件掉落速度，数值越大掉落越快")]
        public float fallSpeed = 3.0f;
        [Range(0f, 1f)]
        [Tooltip("炸弹生成比例（0 ~ 1）")]
        public float bombRatio = 0.25f;
    }

    [Header("三关卡配置（在此调整每关参数）")]
    public LevelConfig[] levels = new LevelConfig[]
    {
        new LevelConfig { levelName = "Level 1 - Normal", duration = 20f, targetScore = 300,  spawnInterval = 1.0f, fallSpeed = 3.0f, bombRatio = 0.20f },
        new LevelConfig { levelName = "Level 2 - Fast",   duration = 20f, targetScore = 500,  spawnInterval = 0.5f, fallSpeed = 4.5f, bombRatio = 0.35f },
        new LevelConfig { levelName = "Level 3 - Frenzy", duration = 30f, targetScore = 1000, spawnInterval = 0.3f, fallSpeed = 7.0f, bombRatio = 0.50f }
    };

    [Header("UI Elements (TextMeshPro)")]
    public TextMeshProUGUI timerText;       // 倒计时文本
    public TextMeshProUGUI scoreText;       // 分数文本
    public TextMeshProUGUI stageText;       // 当前关卡与目标提示
    public GameObject gameOverPanel;        // 结算面板
    public TextMeshProUGUI resultText;      // 结算面板上的过关/失败提示
    public GameObject nextLevelButton;      // 结算面板上的“下一关”按钮
    public GameObject retryButton;          // 结算面板上的“重试本关”按钮

    [Header("Score Rules")]
    public int applePoints = 100;           // 接到苹果加分
    public int bombPenalty = 50;            // 接到炸弹扣分

    [Header("References")]
    public ItemGenerator itemGenerator;     // 物品生成器引用
    public BasketController basket;         // 篮子控制器引用

    private int currentLevelIndex = 0;
    private float remainingTime;
    private int currentScore = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartLevel(0);
    }

    void Update()
    {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerUI();
            OnLevelTimeUp();
            return;
        }

        UpdateTimerUI();
    }

    public void StartLevel(int levelIndex)
    {
        if (levels == null || levels.Length == 0) return;

        currentLevelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        LevelConfig config = levels[currentLevelIndex];

        remainingTime = config.duration;
        currentScore = 0;
        isGameOver = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (itemGenerator != null)
        {
            itemGenerator.spawnInterval = config.spawnInterval;
            itemGenerator.fallSpeed = config.fallSpeed;
            itemGenerator.bombRatio = config.bombRatio;
            itemGenerator.StartGenerating();
        }

        if (basket != null)
            basket.SetGameActive(true);

        UpdateStageUI(config);
        UpdateScoreUI();
        UpdateTimerUI();
    }

    public void StartNextLevel()
    {
        if (currentLevelIndex + 1 < levels.Length)
        {
            StartLevel(currentLevelIndex + 1);
        }
    }

    public void RetryCurrentLevel()
    {
        StartLevel(currentLevelIndex);
    }

    public void NotifyAppleCaught()
    {
        if (isGameOver) return;
        currentScore += applePoints;
        UpdateScoreUI();
    }

    public void NotifyBombCaught()
    {
        if (isGameOver) return;
        currentScore = Mathf.Max(0, currentScore - bombPenalty);
        UpdateScoreUI();
    }

    private void UpdateStageUI(LevelConfig config)
    {
        if (stageText != null)
        {
            stageText.text = config.levelName + " (" + (currentLevelIndex + 1) + "/" + levels.Length + ")";
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            int target = levels[currentLevelIndex].targetScore;
            if (target > 0)
                scoreText.text = "Score: " + currentScore + " / " + target;
            else
                scoreText.text = "Score: " + currentScore;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + remainingTime.ToString("F1") + "s";
    }

    private void OnLevelTimeUp()
    {
        isGameOver = true;

        if (itemGenerator != null)
            itemGenerator.StopGenerating();

        if (basket != null)
            basket.SetGameActive(false);

        ClearRemainingItems();

        LevelConfig config = levels[currentLevelIndex];
        bool isPassed = currentScore >= config.targetScore;
        bool hasNextLevel = currentLevelIndex + 1 < levels.Length;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (nextLevelButton != null)
            nextLevelButton.SetActive(isPassed && hasNextLevel);

        if (retryButton != null)
            retryButton.SetActive(true);

        if (resultText != null)
        {
            if (isPassed && !hasNextLevel)
                resultText.text = "All Clear! Final Score: " + currentScore;
            else if (isPassed)
                resultText.text = config.levelName + " Clear! Score: " + currentScore;
            else
                resultText.text = "Failed! Need " + config.targetScore + " pts";
        }
    }

    private void ClearRemainingItems()
    {
        foreach (GameObject apple in GameObject.FindGameObjectsWithTag("Apple"))
            Destroy(apple);
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("Bomb"))
            Destroy(bomb);
    }
}