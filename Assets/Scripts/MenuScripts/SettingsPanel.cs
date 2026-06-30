using UnityEngine;
using UnityEngine.UI;

// Difficulty selector for the new Settings panel in MainMenu. Each button calls one of
// SetEasy/SetMedium/SetHard, which persists the choice via DifficultyManager and
// highlights the active option. Modeled on the panel-bound MenuScripts style.
public class SettingsPanel : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    [Header("Highlight Colors")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);

    private void OnEnable()
    {
        Highlight(DifficultyManager.Current);
    }

    public void SetEasy() { Apply(Difficulty.Easy); }
    public void SetMedium() { Apply(Difficulty.Medium); }
    public void SetHard() { Apply(Difficulty.Hard); }

    private void Apply(Difficulty difficulty)
    {
        DifficultyManager.Current = difficulty;
        Highlight(difficulty);
    }

    private void Highlight(Difficulty difficulty)
    {
        SetButtonColor(easyButton, difficulty == Difficulty.Easy);
        SetButtonColor(mediumButton, difficulty == Difficulty.Medium);
        SetButtonColor(hardButton, difficulty == Difficulty.Hard);
    }

    private void SetButtonColor(Button button, bool selected)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = selected ? selectedColor : normalColor;
        }
    }
}
