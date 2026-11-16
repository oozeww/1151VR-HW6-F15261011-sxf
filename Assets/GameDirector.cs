using UnityEngine;
using TMPro; // 使用 TextMeshPro 显示 UI

public class GameDirector : MonoBehaviour
{
    // 单例模式，方便篮子直接通知导演
    public static GameDirector Instance { get; private set; }

    [Header("UI Elements (TextMeshPro)")]
    public TextMeshProUGUI timerText;      // 倒计时文本
    public TextMeshProUGUI scoreText;      // 分数文本
    public TextMeshProUGUI stageText;      // (可选) 当前难度阶段提示
    public GameObject gameOverPanel;       // (可选) 游戏结束面板

    [Header("Game Rules")]
    public float gameDuration = 60f;       // 游戏总时长（秒）
    public int applePoints = 100;          // 接到苹果加分
    public int bombPenalty = 50;           // 接到炸弹扣分

    [Header("References")]
    public ItemGenerator itemGenerator;    // 物品生成器引用
    public BasketController basket;        // 篮子控制器引用

    private float remainingTime;
    private int currentScore = 0;
    private bool isGameOver = false;
    private int currentStage = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        remainingTime = gameDuration;
        currentScore = 0;
        isGameOver = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
        UpdateTimerUI();
        ApplyDifficultyStage(1); // 初始化为第 1 阶段难度
    }

    void Update()
    {
        if (isGameOver) return;

        // 1. 倒计时逻辑
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerUI();
            StopGame(); // 时间结束，停止游戏
            return;
        }

        UpdateTimerUI();

        // 2. 挑战：根据剩余时间自动调整关卡难度
        UpdateDifficultyByTime();
    }

    // --- 接收篮子通知的方法 ---
    public void NotifyAppleCaught()
    {
        if (isGameOver) return;
        currentScore += applePoints;
        UpdateScoreUI();
    }

    public void NotifyBombCaught()
    {
        if (isGameOver) return;
        // 扣分，最低扣到 0 分（如果不希望出现负分）
        currentScore = Mathf.Max(0, currentScore - bombPenalty);
        UpdateScoreUI();
    }

    // --- UI 更新 ---
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            // F1 显示一位小数（如 59.8s），若只需整数可改为 "00"
            timerText.text = "Time: " + remainingTime.ToString("F1") + "s";
    }

    // --- 关卡设计：三阶段难度调整 ---
    private void UpdateDifficultyByTime()
    {
        if (remainingTime > 40f && currentStage != 1)
        {
            ApplyDifficultyStage(1);
        }
        else if (remainingTime <= 40f && remainingTime > 20f && currentStage != 2)
        {
            ApplyDifficultyStage(2);
        }
        else if (remainingTime <= 20f && currentStage != 3)
        {
            ApplyDifficultyStage(3);
        }
    }

    private void ApplyDifficultyStage(int stage)
    {
        currentStage = stage;
        if (itemGenerator == null) return;

        switch (stage)
        {
            case 1: // 前 20 秒：热身阶段
                itemGenerator.spawnInterval = 1.0f;
                itemGenerator.bombRatio = 0.25f;
                if (stageText != null) stageText.text = "Stage 1: Normal";
                break;

            case 2: // 40s ~ 20s：节奏加快，炸弹变多
                itemGenerator.spawnInterval = 0.7f;
                itemGenerator.bombRatio = 0.40f;
                if (stageText != null) stageText.text = "Stage 2: Fast!";
                break;

            case 3: // 最后 20 秒：狂热冲刺
                itemGenerator.spawnInterval = 0.45f;
                itemGenerator.bombRatio = 0.55f;
                if (stageText != null) stageText.text = "Stage 3: Frenzy!!";
                break;
        }
    }

    // --- 时间结束后停止游戏 ---
    private void StopGame()
    {
        isGameOver = true;

        // 1. 停止生成新物品
        if (itemGenerator != null)
            itemGenerator.StopGenerating();

        // 2. 禁止篮子继续移动与接取
        if (basket != null)
            basket.SetGameActive(false);

        // 3. 清除场上还在半空下落的残留苹果和炸弹
        ClearRemainingItems();

        // 4. 显示结束画面（如果已配置）
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Time's Up! Final Score: " + currentScore);
    }

    private void ClearRemainingItems()
    {
        foreach (GameObject apple in GameObject.FindGameObjectsWithTag("Apple"))
            Destroy(apple);
        foreach (GameObject bomb in GameObject.FindGameObjectsWithTag("Bomb"))
            Destroy(bomb);
    }
}