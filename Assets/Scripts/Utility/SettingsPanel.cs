using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    GameController m_gameController;
    TouchController m_touchController;

    public Slider m_dragDistanceSlider;
    public Slider m_swipeDistanceSlider;
    public Slider m_dragSpeedSlider;
    public Slider m_gameDifficultySlider;

    public Toggle m_toggleDiagnostic;
    // Start is called before the first frame update
    void Start()
    {
        m_gameController = FindObjectOfType<GameController>().GetComponent<GameController>();
        m_touchController = FindObjectOfType<TouchController>().GetComponent<TouchController>();

        if(m_dragDistanceSlider != null)
        {
            m_dragDistanceSlider.minValue = 20;
            m_dragDistanceSlider.maxValue = 150;
            m_dragDistanceSlider.value = 50;
            ShowSliderValue(m_dragDistanceSlider);
        }

        if(m_swipeDistanceSlider != null)
        { 
            m_swipeDistanceSlider.minValue = 10;
            m_swipeDistanceSlider.maxValue = 250;
            m_swipeDistanceSlider.value = 10;
            ShowSliderValue(m_swipeDistanceSlider);
        }

        if(m_dragSpeedSlider != null)
        {
            m_dragSpeedSlider.minValue = 0.05f;
            m_dragSpeedSlider.value = 0.1f;
            m_dragSpeedSlider.maxValue = 0.5f;
            ShowSliderValue(m_dragSpeedSlider);
        }

        if(m_gameDifficultySlider != null)
        {
            m_gameDifficultySlider.minValue = 0.05f;
            m_gameDifficultySlider.maxValue = 1f;
            m_gameDifficultySlider.value = 0.3f;
            ShowSliderValue(m_gameDifficultySlider);
        }
        if (m_toggleDiagnostic != null && m_touchController != null)
        {
            m_touchController.m_useDiagnostic = m_toggleDiagnostic.isOn;
        }
    }

    public void UpdatePanel()
    {
        if (m_touchController != null) { 

            if (m_dragDistanceSlider != null)
            {
                m_touchController.m_minDragDistance = (int) m_dragDistanceSlider.value;
                ShowSliderValue(m_dragDistanceSlider);
            }

            if (m_swipeDistanceSlider != null)
            {
                m_touchController.m_minSwipeDistance = (int) m_swipeDistanceSlider.value;
                ShowSliderValue(m_swipeDistanceSlider);
            }

            if(m_toggleDiagnostic != null)
            {
                m_touchController.m_useDiagnostic = m_toggleDiagnostic.isOn;
            }
        }

        if(m_gameController != null)
        {
            if(m_dragSpeedSlider != null)
            {
                m_gameController.m_minTimeToDrag = m_dragSpeedSlider.value;
                ShowSliderValue(m_dragSpeedSlider);
            }

            if(m_gameDifficultySlider != null)
            {
                m_gameController.m_dropInterval = m_gameDifficultySlider.value;
                m_gameController.CalculateDropSpeed();
                ShowSliderValue(m_gameDifficultySlider);
            }

        }
    }

    public void ShowSliderValue(Slider currentSlider)
    {
        string sliderValue = currentSlider.value.ToString("F2");
        currentSlider.GetComponentInChildren<Text>().text = sliderValue;
    }

}
