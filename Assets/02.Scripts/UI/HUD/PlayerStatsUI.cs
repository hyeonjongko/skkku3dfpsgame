using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Image _healthimage;

    private float _lastHealth = 0;


    private void Update()
    {
        _healthSlider.value = _stats.Health.Value / _stats.Health.MaxValue;
        _staminaSlider.value = _stats.Stamina.Value / _stats.Stamina.MaxValue;

        if(_lastHealth != _stats.Health.Value)
        {
            _lastHealth = _stats.Health.Value;
            _healthimage.fillAmount = _stats.Health.Value / _stats.Health.MaxValue;
        }
    }
}
