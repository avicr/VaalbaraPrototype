using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using System;

public struct InputContainer
{
    public bool Attack2Pressed;
    public bool Attack2Released;

    public bool AttackPressed;    
    public bool AttackReleased;

    public bool PreviousWeaponPressed;
    public bool NextWeaponPressed;

    public Vector2 MovementAxis;

    
    public bool DownHeld;

    public AxisState AxisStateX;

    public void Reset()
    {
        this.Attack2Pressed = false;
        this.Attack2Released = false;
        this.AttackPressed = false;        
        this.AttackReleased = false;
        this.PreviousWeaponPressed = false;
        this.NextWeaponPressed = false;
        this.MovementAxis = Vector3.zero;        
        this.DownHeld = false;        
    }

    public bool WasSomethingPressed()
    {
        bool pressed = Attack2Pressed || Attack2Released
            || AttackPressed || PreviousWeaponPressed || NextWeaponPressed;
        pressed = pressed || MovementAxis.x != 0f || MovementAxis.y != 0f || DownHeld;
        return pressed;
    }
}

public struct TouchContainer
{
    public bool TouchLeft;
    public bool TouchRight;
    public bool TouchUp;
    public bool TouchDown;
    public bool TouchAttack;
    public bool TouchAttack2;
}

public enum eCharacter
{
    NotSelected,
    Josh,
    Crispin,
    Scott,
    Trent,
    Eddie
};

public enum AxisState
{
    NoChange,
    Zero,
    Negative,
    Positive
}

[Serializable]
public struct PlayerInfo 
{
    public int Score;
    public int PlayerNumber;
    public bool HasJoined;
    public eCharacter SelectedCharacter;

}

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    public PlayerInfo Info;
    protected int PlayerNumber;
    public KeyCode JumpingKey;
    public KeyCode AttackingKey;
    public KeyCode MoveLeftKey;
    public KeyCode MoveRightKey;
    public KeyCode DownKey;
    public KeyCode UpKey;
    public KeyCode PreviousWeaponKey;
    public KeyCode NextWeaponKey;

    public PlayerController Player;

    public bool IsAIControlled;

    protected Vector2 InitialTouchPosition;    
    public int DeadZone;
           
    public bool FixedJoystick;
    public bool RenderJoystick;

    public TMP_InputField DeadZoneTextField;

    protected AxisState LastAxisXState;

    InputContainer inputContainer;

    void Start()
    {
        if (Player != null)
        {
            Player.inputContainer.Reset();
        }
        else
        {
            PlayerNumber = 0;
        }
        //DeadZoneTextField.text = DeadZone.ToString();
    }

    public InputContainer GetCachedInput()
    {
        return inputContainer;
    }

    public void OnFixedJoystickChanged(bool Value)
    {
        FixedJoystick = Value;
    }

    public void OnRenderJoystickChanged(bool Value)
    {
        RenderJoystick = Value;
    }

    public void OnSetDeadZone(string DeadZoneString)
    {
        DeadZone = int.Parse(DeadZoneString);
    }

    // Update is called once per frame
    void Update ()
    {
        GetInput();
        if (!IsAIControlled && Player != null)
        {
            Player.inputContainer = GetCachedInput();
        }
    }

    private void OnGUI()
    {
        if (RenderJoystick)
        {
            Drawing.DrawCircle(new Vector2(InitialTouchPosition.x, 1080 - InitialTouchPosition.y), DeadZone - 10, Color.red, 10, 9);
        }
    }

    private InputContainer GetInput()
    {
        inputContainer = new InputContainer();
        inputContainer.Reset();

        // Check Movement

        float movementX = Input.GetAxisRaw(InputUtility.GetXAxisName(0));
        float movementY = Input.GetAxisRaw(InputUtility.GetYAxisName(0));

        if (Player != null)
        {
            movementX = Input.GetAxisRaw(InputUtility.GetXAxisName(Player.PlayerNumber));
            movementY = Input.GetAxisRaw(InputUtility.GetYAxisName(Player.PlayerNumber));
        }



        TouchContainer Touch = GetTouchInput();

        if (Input.GetKey(UpKey) || Touch.TouchUp)
        {
            movementY = 1f;
        }
        else if (Input.GetKey(DownKey) || Touch.TouchDown)
        {
            movementY = -1f;
        }

        if (Input.GetKey(MoveLeftKey) || Touch.TouchLeft)
        {
            movementX = -1f;
        }
        else if (Input.GetKey(MoveRightKey) || Touch.TouchRight)
        {
            movementX = 1f;
        }        

        inputContainer.MovementAxis = new Vector2(movementX, movementY); // TODO - handle Y axis for shooting special weapons        

        // Change to deadzone
        if (movementX == 0)
        {
            inputContainer.AxisStateX = AxisState.Zero;
        }
        else
        {
            if (Mathf.Sign(movementX) == 1)
            {
                inputContainer.AxisStateX = AxisState.Positive;
            }
            else
            {
                inputContainer.AxisStateX = AxisState.Negative;
            }
        }

        AxisState TempAxisXState = inputContainer.AxisStateX;

        if (inputContainer.AxisStateX == LastAxisXState)
        {
            inputContainer.AxisStateX = AxisState.NoChange;
        }

        LastAxisXState = TempAxisXState;


        // Check Attack2ing
        inputContainer.Attack2Pressed = Input.GetButtonDown(InputUtility.GetJumpButtonName(PlayerNumber))
            || Input.GetKeyDown(JumpingKey) || Touch.TouchAttack2;
       
        inputContainer.Attack2Released = Input.GetButtonUp(InputUtility.GetJumpButtonName(PlayerNumber))
            || Input.GetKeyUp(JumpingKey);

        // Check Shooting
        string shootString = InputUtility.GetShootButtonName(PlayerNumber);
        inputContainer.AttackPressed = Input.GetButtonDown(shootString)
            || Input.GetKeyDown(AttackingKey) || Touch.TouchAttack;
        
        inputContainer.AttackReleased = Input.GetButtonUp(shootString)
            || Input.GetKeyUp(AttackingKey);

        // Check weapon switching
        inputContainer.PreviousWeaponPressed = /*Input.GetButtonDown(InputUtility.GetPreviousWeaponButtonName(Player.PlayerNumber))
            || */Input.GetKeyDown(PreviousWeaponKey);

        inputContainer.NextWeaponPressed = /*Input.GetButtonDown(InputUtility.GetNextWeaponButtonName(Player.PlayerNumber))
            || */Input.GetKeyDown(NextWeaponKey);
       

        return inputContainer;
    }

    //protected TouchContainer GetTouchInput()
    //{
    //    TouchContainer TouchResults = new TouchContainer();

    //    if (Input.GetMouseButton(0))
    //    {

    //        Vector2 deltaPosition = (Vector2)Input.mousePosition - InitialTouchPosition;
    //        Vector2 LastTouchPosition = InitialTouchPosition;
    //        Vector2 NewPosition = InitialTouchPosition;
    //        if (deltaPosition.x > DeadZone)
    //        {
    //            //InitialTouchPosition.x = Input.GetTouch(0).position.x - (deltaPosition.x > 0 ?  -DeadZone : -DeadZone);
    //            NewPosition.x = Input.mousePosition.x - DeadZone - 1;

    //        }

    //        if (deltaPosition.x < -DeadZone)
    //        {
    //            //InitialTouchPosition.x = Input.GetTouch(0).position.x - (deltaPosition.x > 0 ?  -DeadZone : -DeadZone);
    //            NewPosition.x = Input.mousePosition.x + DeadZone + 1;

    //        }

    //        if (deltaPosition.y > DeadZone)
    //        {
    //            //InitialTouchPosition.y = Input.GetTouch(0).position.y - (deltaPosition.y > 0 ? -DeadZone : DeadZone);
    //            NewPosition.y = Input.mousePosition.y - DeadZone - 1;
    //        }

    //        if (deltaPosition.y < -DeadZone)
    //        {
    //            //InitialTouchPosition.y = Input.GetTouch(0).position.y - (deltaPosition.y > 0 ? -DeadZone : DeadZone);
    //            NewPosition.y = Input.mousePosition.y + DeadZone + 1;
    //        }

    //        InitialTouchPosition = NewPosition;

    //        Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(InitialTouchPosition.ToString());
    //        //Vector2 DeltaTouch = Input.GetTouch(0).position - InitialTouchPosition;
    //        //bTouchLeft = DeltaTouch.x < 0;
    //        //bTouchRight = DeltaTouch.x > 0;
    //        //bTouchUp = DeltaTouch.y > 0;
    //        //bTouchDown = DeltaTouch.y < 0;

    //        float Angle = GetAngleTo(InitialTouchPosition, Input.mousePosition);

    //        TouchResults.TouchLeft = deltaPosition.x < -DeadZone && Angle >= 120 && Angle <= 240;
    //        TouchResults.TouchRight = deltaPosition.x > DeadZone && ((Angle >= 300 && Angle <= 360) || (Angle >= 0 && Angle <= 60));

    //        TouchResults.TouchUp = deltaPosition.y > DeadZone && Angle >= 30 && Angle <= 150;
    //        TouchResults.TouchDown = deltaPosition.y < -DeadZone && Angle >= 210 && Angle <= 330;
    //        Debug.Log(deltaPosition + " " + Angle);
    //    }

    //    if (Input.GetMouseButtonDown(0) ||(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    //    {
    //        InitialTouchPosition = Input.mousePosition;
    //    }

    //    if (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Ended)
    //    {
    //        TouchResults.TouchAttack = true;
    //    }

    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && InitialTouchPosition == Input.GetTouch(0).position)
    //    {
    //        TouchResults.TouchAttack = true;
    //    }

    //    if (/*Input.GetMouseButtonDown(1) || */(Input.touchCount == 3 && Input.GetTouch(2).phase == TouchPhase.Ended))
    //    {
    //        DeadZone--;
    //        Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
    //    }

    //    if (/*Input.GetMouseButtonDown(0) || */(Input.touchCount == 4 && Input.GetTouch(2).phase == TouchPhase.Ended))
    //    {
    //        DeadZone++;
    //        Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
    //    }
    //    return TouchResults;
    //}

    protected TouchContainer GetTouchInput()
    {
        TouchContainer TouchResults = new TouchContainer();
        Vector2 CurrentTouchPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        if (/*Input.GetMouseButton(0) ||*/ (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)))
        {            
            Vector2 deltaPosition = CurrentTouchPosition - InitialTouchPosition;            
            Vector2 NewPosition = InitialTouchPosition;            

            if (!FixedJoystick)
            {
                if (deltaPosition.x > DeadZone)
                {
                    NewPosition.x = CurrentTouchPosition.x - DeadZone - 1;
                }

                if (deltaPosition.x < -DeadZone)
                {
                    NewPosition.x = CurrentTouchPosition.x + DeadZone + 1;
                }

                if (deltaPosition.y > DeadZone)
                {
                    NewPosition.y = CurrentTouchPosition.y - DeadZone - 1;
                }

                if (deltaPosition.y < -DeadZone)
                {
                    NewPosition.y = CurrentTouchPosition.y + DeadZone + 1;
                }

                InitialTouchPosition = NewPosition;
            }

            float Angle = GetAngleTo(InitialTouchPosition, CurrentTouchPosition);

            TouchResults.TouchLeft = deltaPosition.x < -DeadZone && Angle >= 120 && Angle <= 240;
            TouchResults.TouchRight = deltaPosition.x > DeadZone && ((Angle >= 300 && Angle <= 360) || (Angle >= 0 && Angle <= 60));

            TouchResults.TouchUp = deltaPosition.y > DeadZone && Angle >= 30 && Angle <= 150;
            TouchResults.TouchDown = deltaPosition.y < -DeadZone && Angle >= 210 && Angle <= 330;
        }

        if (/*Input.GetMouseButtonDown(0) || */(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            InitialTouchPosition = CurrentTouchPosition;
            //Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(InitialTouchPosition.ToString());
        }

        if (/*Input.GetMouseButtonUp(1) || */(Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Ended))
        {
            if (Input.GetTouch(1).position.y < Screen.height / 2)
            {
                TouchResults.TouchAttack = true;
            }
            else
            {
                TouchResults.TouchAttack2 = true;
            }
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && Vector2.Distance(Input.GetTouch(0).position,InitialTouchPosition) <= DeadZone)
        {
            if (Input.GetTouch(0).position.y < Screen.height / 2)
            {
                TouchResults.TouchAttack = true;
            }
            else
            {
                TouchResults.TouchAttack2 = true;
            }

            if (Input.GetTouch(0).position.x >= Screen.width / 2)
            {
                //Player.ChangeDirection(0);
            }
            else
            {
                //Player.ChangeDirection(1);

            }
        }

        if (/*Input.GetMouseButtonDown(1) || */(Input.touchCount == 3 && Input.GetTouch(2).phase == TouchPhase.Ended))
        {
            DeadZone--;
            //Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
        }

        if (/*Input.GetMouseButtonDown(0) || */(Input.touchCount == 4 && Input.GetTouch(2).phase == TouchPhase.Ended))
        {
            DeadZone++;
            //Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
        }
        return TouchResults;
    }

    //protected TouchContainer GetTouchInput()
    //{
    //    TouchContainer TouchResults = new TouchContainer();

    //    if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved))
    //    {

    //        Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
    //        if (deltaPosition.x > DeadZone || deltaPosition.x < -DeadZone)
    //        {
    //            //InitialTouchPosition.x = Input.GetTouch(0).position.x - (deltaPosition.x > 0 ?  -DeadZone : -DeadZone);
    //            InitialTouchPosition = Input.GetTouch(0).position - deltaPosition;

    //        }

    //        if (deltaPosition.y > DeadZone || deltaPosition.y < -DeadZone)
    //        {
    //            //InitialTouchPosition.y = Input.GetTouch(0).position.y - (deltaPosition.y > 0 ? -DeadZone : DeadZone);
    //            InitialTouchPosition = Input.GetTouch(0).position - deltaPosition;
    //        }
            
    //        //Vector2 DeltaTouch = Input.GetTouch(0).position - InitialTouchPosition;
    //        //bTouchLeft = DeltaTouch.x < 0;
    //        //bTouchRight = DeltaTouch.x > 0;
    //        //bTouchUp = DeltaTouch.y > 0;
    //        //bTouchDown = DeltaTouch.y < 0;

    //        float Angle = GetAngleTo(InitialTouchPosition, Input.GetTouch(0).position);

    //        TouchResults.TouchLeft = Angle >= 120 && Angle <= 240;
    //        TouchResults.TouchRight = ((Angle >= 300 && Angle <= 360) || (Angle >= 0 && Angle <= 60));

    //        TouchResults.TouchUp = Angle >= 30 && Angle <= 150;
    //        TouchResults.TouchDown = Angle >= 210 && Angle <= 330;    
    //    }

    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        InitialTouchPosition = Input.GetTouch(0).position;
    //    }

    //    if (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Ended)
    //    {
    //        TouchResults.TouchAttack = true;
    //    }

    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && InitialTouchPosition == Input.GetTouch(0).position)
    //    {
    //        TouchResults.TouchAttack = true;
    //    }

    //    if (/*Input.GetMouseButtonDown(1) || */(Input.touchCount == 3 && Input.GetTouch(2).phase == TouchPhase.Ended))
    //    {
    //        DeadZone--;
    //        Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
    //    }

    //    if (/*Input.GetMouseButtonDown(0) || */(Input.touchCount == 4 && Input.GetTouch(2).phase == TouchPhase.Ended))
    //    {
    //        DeadZone++;
    //        Player.TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(DeadZone.ToString());
    //    }
    //    return TouchResults;
    //}
    public float GetAngleTo(Vector2 A, Vector2 B)
    {
        Vector2 Direction = B - A;
        float Angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        if (Angle < 0f) Angle += 360f;

        return Angle;
    }
}
