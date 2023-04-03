using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphicSettingsUI : MonoBehaviour {
    private const string PLAYER_PREFS_GRAPHIC_QUALITY_KEY = "GraphicQuality";
    [SerializeField] private Button graphicQualityButton;
    [SerializeField] private TextMeshProUGUI graphicQualityText;
    [SerializeField] List<RenderPipelineAsset> renderPipelineAssetsList;

    private int currentGraphicQuality;
    private void Awake() {
        currentGraphicQuality = renderPipelineAssetsList.Count - 1;

        currentGraphicQuality = PlayerPrefs.GetInt(PLAYER_PREFS_GRAPHIC_QUALITY_KEY, currentGraphicQuality);
        
        SetGraphicQuality();
    }
    void Start() {

        graphicQualityButton.onClick.AddListener(() => {
            currentGraphicQuality++;
            if (currentGraphicQuality > renderPipelineAssetsList.Count - 1) {
                currentGraphicQuality = 0;
            }
            SetGraphicQuality();
            UpdateVisual();
        });
        UpdateVisual();
    }
    private void SetGraphicQuality() {
        QualitySettings.SetQualityLevel(currentGraphicQuality);
        QualitySettings.renderPipeline = renderPipelineAssetsList[currentGraphicQuality];
        PlayerPrefs.SetInt(PLAYER_PREFS_GRAPHIC_QUALITY_KEY, currentGraphicQuality);
    }
    private void UpdateVisual() {
        graphicQualityText.text = "Graphic Quality: " + (GraphicQuality)currentGraphicQuality;
    }
    enum GraphicQuality {
        Low, Medium, High
    }
}
