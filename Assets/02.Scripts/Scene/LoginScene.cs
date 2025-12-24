using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LoginScene : MonoBehaviour
{
    // 로그인씬 (로그인/회원가입) -> 게임씬

    private enum SceneMode
    {
        Login,
        Register
    }

    private SceneMode _mode = SceneMode.Login;

    // 마지막 로그인 정보 저장용 키
    private const string LAST_ID_KEY = "LastLoginId";
    private const string LAST_PASSWORD_KEY = "LastLoginPassword";

    // 정규표현식 패턴
    // 이메일 패턴: 기본적인 이메일 형식 검증
    private const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    // 비밀번호 패턴: 영어/숫자/특수문자만 허용
    private const string PASSWORD_ALLOWED_CHARS = @"^[a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+$";

    // 비밀번호 확인 오브젝트
    [SerializeField] private GameObject _passwordCofirmObject;
    [SerializeField] private Button _gotoRegisterButton;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _gotoLoginButton;
    [SerializeField] private Button _registerButton;

    [SerializeField] private TextMeshProUGUI _messageTextUI;

    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _passwordConfirmInputField;

    private void Start()
    {
        AddButtonEvents();
        LoadLastLoginInfo();
        Refresh();
    }

    private void AddButtonEvents()
    {
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _loginButton.onClick.AddListener(Login);
        _gotoLoginButton.onClick.AddListener(GotoLogin);
        _registerButton.onClick.AddListener(Register);
    }

    private void LoadLastLoginInfo()
    {
        // 저장된 마지막 로그인 정보가 있으면 자동으로 채워넣기
        if (PlayerPrefs.HasKey(LAST_ID_KEY))
        {
            _idInputField.text = PlayerPrefs.GetString(LAST_ID_KEY);
        }

        if (PlayerPrefs.HasKey(LAST_PASSWORD_KEY))
        {
            _passwordInputField.text = PlayerPrefs.GetString(LAST_PASSWORD_KEY);
        }
    }

    private void SaveLastLoginInfo(string id, string password)
    {
        // 마지막 로그인 정보 저장
        PlayerPrefs.SetString(LAST_ID_KEY, id);
        PlayerPrefs.SetString(LAST_PASSWORD_KEY, password);
        PlayerPrefs.Save(); // 즉시 디스크에 저장하여 유니티 에디터 재시작 시에도 유지
    }

    private void OnApplicationQuit()
    {
        // 앱 종료 시에도 저장 보장
        PlayerPrefs.Save();
    }

    private void Refresh()
    {
        // 2차 비밀번호 오브젝트는 회원가입 모드일때만 노출
        _passwordCofirmObject.SetActive(_mode == SceneMode.Register);
        _gotoRegisterButton.gameObject.SetActive(_mode == SceneMode.Login);
        _loginButton.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoLoginButton.gameObject.SetActive(_mode == SceneMode.Register);
        _registerButton.gameObject.SetActive(_mode == SceneMode.Register);
    }

    /// <summary>
    /// 이메일 형식 검증
    /// </summary>
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return Regex.IsMatch(email, EMAIL_PATTERN);
    }

    /// <summary>
    /// 비밀번호 검증
    /// - 7자 이상 20자 이하
    /// - 영어 대문자 1개 이상
    /// - 영어 소문자 1개 이상
    /// - 특수문자 1개 이상
    /// - 영어/숫자/특수문자만 가능
    /// </summary>
    private bool IsValidPassword(string password, out string errorMessage)
    {
        errorMessage = "";

        if (string.IsNullOrEmpty(password))
        {
            errorMessage = "패스워드를 입력해주세요.";
            return false;
        }

        // 길이 검증 (7자 이상 20자 이하)
        if (password.Length < 7 || password.Length > 20)
        {
            errorMessage = "패스워드는 7자 이상 20자 이하여야 합니다.";
            return false;
        }

        // 허용된 문자만 사용했는지 검증 (영어/숫자/특수문자)
        if (!Regex.IsMatch(password, PASSWORD_ALLOWED_CHARS))
        {
            errorMessage = "패스워드는 영어, 숫자, 특수문자만 사용 가능합니다.";
            return false;
        }

        // 대문자 1개 이상 포함
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            errorMessage = "패스워드는 영어 대문자를 1개 이상 포함해야 합니다.";
            return false;
        }

        // 소문자 1개 이상 포함
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            errorMessage = "패스워드는 영어 소문자를 1개 이상 포함해야 합니다.";
            return false;
        }

        // 특수문자 1개 이상 포함
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
        {
            errorMessage = "패스워드는 특수문자를 1개 이상 포함해야 합니다.";
            return false;
        }

        return true;
    }

    private void Login()
    {
        // 로그인
        // 1. 아이디 입력을 확인한다.
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        // 2. 비밀번호 입력을 확인한다.
        string password = _passwordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "패스워드를 입력해주세요.";
            return;
        }

        // 3. 실제 저장된 아이디-비밀번호 계정이 있는지 확인한다.
        // 3-1. 아이디가 있는지 확인한다.
        if (!PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "아이디를 확인해주세요.";
            return;
        }

        string myPassword = PlayerPrefs.GetString(id);
        if (myPassword != password)
        {
            _messageTextUI.text = "비밀번호를 확인해주세요.";
            return;
        }

        // 4. 로그인 성공 - 마지막 로그인 정보 저장
        SaveLastLoginInfo(id, password);

        // 5. 씬 이동
        SceneManager.LoadScene("LoadingScene");
    }

    private void Register()
    {
        // 회원가입
        // 1. 아이디(이메일) 입력을 확인한다.
        string id = _idInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "이메일을 입력해주세요.";
            return;
        }

        // 1-1. 이메일 형식 검증
        if (!IsValidEmail(id))
        {
            _messageTextUI.text = "올바른 이메일 형식이 아닙니다.";
            return;
        }

        // 2. 비밀번호 입력을 확인한다.
        string password = _passwordInputField.text;
        string passwordError;
        if (!IsValidPassword(password, out passwordError))
        {
            _messageTextUI.text = passwordError;
            return;
        }

        // 3. 비밀번호 확인 입력을 확인한다.
        string password2 = _passwordConfirmInputField.text;
        if (string.IsNullOrEmpty(password2) || password != password2)
        {
            _messageTextUI.text = "패스워드 확인이 일치하지 않습니다.";
            return;
        }

        // 4. 실제 저장된 아이디-비밀번호 계정이 있는지 확인한다.
        // 4-1. 아이디가 있는지 확인한다.
        if (PlayerPrefs.HasKey(id))
        {
            _messageTextUI.text = "이미 사용 중인 이메일입니다.";
            return;
        }

        // 5. 회원가입 성공
        PlayerPrefs.SetString(id, password);
        _messageTextUI.text = "회원가입이 완료되었습니다!";

        // 입력 필드 초기화 후 로그인 화면으로 이동
        _idInputField.text = "";
        _passwordInputField.text = "";
        _passwordConfirmInputField.text = "";

        GotoLogin();
    }

    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        _messageTextUI.text = "";
        Refresh();
    }

    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        _messageTextUI.text = "";
        Refresh();
    }
}
