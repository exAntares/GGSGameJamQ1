using HalfBlind.ScriptableVariables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainController : MonoBehaviour {
    public ScriptableGameEvent _onHeroDied;

    public ScriptableGameEvent _onPlayerLost;
    public ScriptableGameEvent _onPlayerWon;

    public GlobalFloat _currentLevel;

    private void Awake() {
        _onHeroDied.AddListener(OnHeroDied);

        _onPlayerLost.AddListener(OnPlayerLost);
        _onPlayerWon.AddListener(OnPlayerWon);
    }

    private void OnDestroy() {
        _onHeroDied.RemoveListener(OnHeroDied);

        _onPlayerLost.RemoveListener(OnPlayerLost);
        _onPlayerWon.RemoveListener(OnPlayerWon);
    }

    private void OnPlayerWon() {
        _currentLevel.Value = _currentLevel.Value + 1;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void OnPlayerLost() {
        if (_currentLevel.Value > 0) {
            _currentLevel.Value = _currentLevel.Value - 1;
        }
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void OnAllEnemiesDied() {
        _onPlayerWon.SendEvent();
    }

    private void OnHeroDied() {
        _onPlayerLost.SendEvent();
    }
}
