using UnityEngine;

public class UI_CombatBar : MonoBehaviour
{
    public float FillAmount;

    private Transform bar;
    private RectTransform barRect;
    private float initalBarX;
    private float barWidth;

    void Awake() {
        bar = transform.Find("BarMask").Find("Bar");
        barRect = bar.GetComponent<RectTransform>();
        barWidth = barRect.rect.width;
        initalBarX = barRect.localPosition.x;
        
        SetBarFill(FillAmount);
    }

    public void SetBarFill(float fillPercent) {
        fillPercent = Mathf.Clamp(fillPercent, 0, 1);
        barRect.localPosition = new Vector3(barWidth * fillPercent - barWidth + initalBarX, 0, 0);

        FillAmount = fillPercent;
    }
}
