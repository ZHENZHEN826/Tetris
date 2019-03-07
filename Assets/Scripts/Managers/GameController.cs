/* TODO: Particle lessons
 *       Android development
 *       Give a while before landshape
 *       Remeber setting
 *       When pause, block should not move
 *       Longer board
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Board m_gameBoard;

    Spawner m_spawner;

    ScoreManager m_scoreManager;

    Shape m_activeShape;

    // ghost for visualization
    Ghost m_ghost;

    Holder m_holder;

    public float m_dropInterval = 0.3f;
    float m_dropIntervalModed;

    float m_timeToDrop;
    /*
    float m_timeToNextKey;
    [Range(0.02f,1f)]
    public float m_keyRepeatRate = 0.25f;
    */

    float m_timeToNextKeyLeftRight;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.075f;


    float m_timeToNextKeyDown;
    [Range(0.01f, 1f)]
    public float m_keyRepeatRateDown = 0.02f;


    float m_timeToNextKeyRotate;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = 0.1f;

    bool m_gameOver = false;

    public GameObject m_gameOverPanel ;

    public IconToggle m_rotIconToggle;

    bool m_clockwise = true;

    public bool m_isPaused = false;

    public GameObject m_pausePanel;

    SoundManager m_soundManager;

    public Text m_diagnosticText3;

    enum Direction {none, left, right, up, down}

    Direction m_dragDirection = Direction.none;
    Direction m_swipeDirection = Direction.none;

    float m_timeToNextDrag;
    float m_timeToNextSwipe;

    bool m_didTap = false;

    [Range(0.05f, 1f)]
    public float m_minTimeToDrag = 0.1f;

    [Range(0.05f, 1f)]
    public float m_minTimeToSwipe = 0.2f; 

    private void OnEnable()
    {
        TouchController.DragEvent += DragHandler;
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.TapEvent += TapHandler;
    }

    private void OnDisable()
    {
        TouchController.DragEvent -= DragHandler;
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.TapEvent -= TapHandler;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        //m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        //m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_gameBoard = FindObjectOfType<Board>();
        m_spawner = FindObjectOfType<Spawner>();
        m_soundManager = FindObjectOfType<SoundManager>();
        m_scoreManager = FindObjectOfType<ScoreManager>();
        m_ghost = FindObjectOfType<Ghost>();
        m_holder = FindObjectOfType<Holder>();


        if (!m_gameBoard)
        {
            Debug.LogWarning("WARNING! There is no board defined");
        }

        if (!m_soundManager)
        {
            Debug.LogWarning("WARNING! There is no Sound Manager defined");
        }

        if (!m_scoreManager)
        {
            Debug.LogWarning("WARNING! There is no score manager defined");
        }

        if (!m_spawner)
        {
            Debug.LogWarning("WARNING! There is no spawner defined");
        }
        else
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);
            if (!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }

        if (m_diagnosticText3)
        {
            m_diagnosticText3.text = "";
        } 
        m_dropIntervalModed = m_dropInterval;

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager)
        {
            return;
        }

        PlayerInput();
    }

    void PlayerInput()
    {
        if ((Input.GetButton("MoveRight") && Time.time > m_timeToNextKeyLeftRight) || (Input.GetButtonDown("MoveRight")))
        {
            MoveRight();
        }
        else if ((Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyLeftRight) || (Input.GetButtonDown("MoveLeft")))
        {
            MoveLeft();
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > m_timeToNextKeyRotate)
        {
            Rotate();
        }
        else if ((Time.time > m_timeToDrop) || (Input.GetButton("MoveDown") && Time.time > m_timeToNextKeyDown))
        {
            MoveDown();
        }
        // Touch Control--------
        else if ((m_dragDirection == Direction.right && Time.time > m_timeToNextDrag) || 
            (m_swipeDirection == Direction.right && Time.time > m_timeToNextSwipe))
        {
            MoveRight();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if ((m_dragDirection == Direction.left && Time.time > m_timeToNextDrag) || 
            (m_swipeDirection == Direction.left && Time.time > m_timeToNextSwipe))
        {
            MoveLeft();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if (m_didTap)
        {
            Rotate();
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        } 
        else if (m_dragDirection == Direction.down && Time.time > m_timeToNextDrag)
        {
            MoveDown();
        }
        else if (m_swipeDirection == Direction.up && Time.time > m_timeToNextSwipe)
        {
            Hold();
        }
        // --------
        else if (Input.GetButtonDown("ToggleRot"))
        {
            ToggleRotDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause ();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }

        m_dragDirection = Direction.none;
        m_swipeDirection = Direction.none;
        m_didTap = false;
    }

    private void MoveDown()
    {
        m_timeToDrop = Time.time + m_dropIntervalModed;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

        m_activeShape.MoveDown();

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            if (m_gameBoard.IsOverLimit(m_activeShape))
            {
                GameOver();
            }
            else
            {
                LandShape();
            }

        }
    }



    private void Rotate()
    {
        //m_activeShape.RotateRight();
        m_activeShape.RotateClockwise(m_clockwise);
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            //m_activeShape.RotateLeft();
            m_activeShape.RotateClockwise(!m_clockwise);
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    private void MoveLeft()
    {
        m_activeShape.MoveLeft();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveRight();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    private void MoveRight()
    {
        m_activeShape.MoveRight();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveLeft();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }

        PlaySound(m_soundManager.m_gameOverSound, 5f);
        PlaySound(m_soundManager.m_gameOverVocalClip, 5f);
    }

    void LandShape()
    {
        // shape lands here
        m_activeShape.MoveUp();

        m_gameBoard.StoreShapeInGrid(m_activeShape);

        if (m_ghost)
        {
            m_ghost.Reset();
        }

        if (m_holder)
        {
            m_holder.m_canRelease = true;
        }

        // spawn a new shape
        m_activeShape = m_spawner.SpawnShape();

        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        m_gameBoard.clearAllRows();

        PlaySound(m_soundManager.m_dropSound, 0.75f);

        if (m_gameBoard.m_completeRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completeRows);

            if (m_scoreManager.m_didLevelUp)
            {
                PlaySound(m_soundManager.m_levelUpVocalClip, 0.75f);
                CalculateDropSpeed();
            }
            else
            {
                if (m_gameBoard.m_completeRows > 1)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
                    PlaySound(randomVocal, 0.75f);
                }
            }

            PlaySound(m_soundManager.m_clearRowSound, 0.75f);
        }
    }

    public void CalculateDropSpeed()
    {
        m_dropIntervalModed = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level - 1) * 0.05f, 0.05f, 1f);
    }

    private void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
    }

    public void Restart()
    {
        Debug.Log("Restart game.");
        Time.timeScale = 1;

        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (m_soundManager.m_fxEnabled && clip  )
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier, 0.05f, 1f));
        }
    }

    public void ToggleRotDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
        {
            m_rotIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);
            if (m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
            }

            Time.timeScale = (m_isPaused) ? 0 : 1;
        }
    }

    public void Hold()
    {
        if (!m_holder)
        {
            return;
        }

        if (!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spawner.SpawnShape();
            PlaySound(m_soundManager.m_holdSound,0.5f);
        }
        else if (m_holder.m_canRelease)
        {
            Shape temp = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(temp);
            PlaySound(m_soundManager.m_holdSound, 0.5f);
        }
        else
        {
            Debug.LogWarning("HOLDER WARNING!!! Wait for cool down!");
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }

        if (m_ghost)
        {
            m_ghost.Reset();
        }

    }

    void DragHandler(Vector2 dragMovement)
    {
        //if (m_diagnosticText3)
        //{
        //    m_diagnosticText3.text = "SwipeEvent Detected";
        //}
        m_dragDirection = GetDirection(dragMovement);
    }

    void SwipeHandler(Vector2 swipeMovement)
    {
        //if (m_diagnosticText3)
        //{
        //    m_diagnosticText3.text = "";
        //}
        m_swipeDirection = GetDirection(swipeMovement);
    }

    void TapHandler(Vector2 tapMovement)
    {
        m_didTap = true;
    }


    Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.none;

        // horizontal
        if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDir = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
        }
        // vertical
        else
        {
            swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
        }

        return swipeDir;
    }

}
