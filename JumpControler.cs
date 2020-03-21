using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpControler : MonoBehaviour
{
    private Data MVData;

    private AnimationCurve jumpCurve;
    private AnimationCurve fallCurve;
    private AnimationCurve secondJumpCurve;
    private JumpRayCast jumpRayCast;

    float finalJumpPosition;
    float MaxJumpHight;
    float jumpBeginningPosition;

    float acctualyTimeFallNew;
    float currentlyMinJumpTime;  // dalem  = 0f ale raczej nie ma to sensu xd
    float veolcityEndCurve; // test narazie czegos
    float fallStartPosition;
    float fallDirectionPosition;
    float fallPositionInCurve;
    float fallCurveVelocity;
    float jumpStartPosition;
    float jumpDirectionPosition;
    float jumpPositionInCurve;
    float jumpCurveVelocity;
    float secondJumpStartPosition;
    float secondJumpDirectionPosition;
    float secondJumpPositionInCurve;
    float secondJumpCurveVelocity;


    float maxMinJumpTime = 0.3f;   // czas po jakim czasie isOnGround sprawdza czy juz wyladowales
    float maxFallDuration = 0.3f; // czas w jakim skonczy sie spadanie(1 etap) (przejdzie cala curve)  
    float maxJumpDuration = 0.5f; // czas w jakim skonczy sie skok (przejdzie cala curve)
    float minJumpDuration = 0.4f; // minimalny czas skoku
    float maxSecondJumpDuration = 0.5f; // czas w jakim skonczy sie drugi skok (przejdzie cala curve)

    float minSecondJumpDuration = 1f; // minimalny czas drugiego skoku
 
    float maxTimeFallNew = .3f; // maksymalna dlugosc spadania (casz), od miejsca w torym zaczal skok     // to do wyjebania raczej      

    int fallStep = 1; // to nie zmieniac // narazie 1 bo nie chce mi sie zmieniac na z 0 xd

    bool isCanDoubleJumpNew = true; // To jest bool do drzewka skilla czy wgl ma wykupione doubleJupmp
    bool isUesedDoubleJumpNew;
    bool isCalculateJumpVarble;
    bool isSelectJBP = true; // nie zmienac;
    bool isBottomWasUpNew;

    bool isSecondJumpIsAlwaseShort = true; //true = drugi skok nie jes przyszluszany i ma zawsze taka sama wyskokosc skoku niezaleznie od dlugosci klikania skoku
    
    public JumpControler(Data _MVData, AnimationCurve jumpCurve, AnimationCurve fallCurve , AnimationCurve secondJumpCurve)
    {
        MVData = _MVData;
        this.jumpCurve = jumpCurve;
        this.fallCurve = fallCurve;
        this.secondJumpCurve = secondJumpCurve;
        jumpRayCast = new JumpRayCast(MVData.gameObject); 
        InicializationJump();
    }

    public bool GetRayCast()
    {
        jumpRayCast.RayCastDeBug();
        return isOnGround();
    }

    public bool IsBottomWasUp()  // ta funkcja odpala sie i tak jak jest klikniete
    {
        if (!InputManager.JumpButton() && !isBottomWasUpNew)
        {
            isBottomWasUpNew = true;
        }
        return isBottomWasUpNew;
    }

    public void SelectJumpBeginningPosition()
    {
        if (isSelectJBP)
        {
            jumpBeginningPosition = InputManager.target.transform.position.y;
            isSelectJBP = false;
        }
    }

    public bool IsExceededMinimumTimeJumpUp()
    {
        if (jumpPositionInCurve <= minJumpDuration && !isUesedDoubleJumpNew)
        {
            return true;
        }
        if(secondJumpPositionInCurve <= minSecondJumpDuration && isUesedDoubleJumpNew)
        {
            return true;
        }
        return false;
    }
    public bool IsMaxmaxTimeJumpUp()
    {   
        if(jumpPositionInCurve >= 1 && !isUesedDoubleJumpNew)
        {
            return true;
        }
        if(secondJumpPositionInCurve >= 1 && isUesedDoubleJumpNew)
        {
            return true;
        }

        return false;
    }

    public bool IsMaxTimeFallEndNew()
    {
        if (acctualyTimeFallNew <= maxTimeFallNew)
        {
            acctualyTimeFallNew += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool isOnGroundWhenJump()
    {
        if (isOnGround() && currentlyMinJumpTime >= maxMinJumpTime)
        {
            IsOnGroundReset();
            return true;
        }
        else if (currentlyMinJumpTime >= maxMinJumpTime)
        {
            return false;
        }
        else
        {
            currentlyMinJumpTime += Time.deltaTime;
            return false;
        }
    }

    public bool IsCanDoubleJump()
    {
        if (InputManager.JumpButton() && isCanDoubleJumpNew && isBottomWasUpNew && !isUesedDoubleJumpNew) // moze dodac && isFall 
        {
            isUesedDoubleJumpNew = true;
            ResetFallParametr(); // acctualyTimeFallNew = 0f;
            ResetSelectJumpBeginningPosition(); 
            ResetCalculateJumpVarble();
            ResetCalculateFallVelocity(); ///
            ResetCalculateJumpVelocity(); ///
            ResetCalculateSecondJumpVelocity(); //
            fallStep = 1;
            return true;
        }
        else
        {
            return false;
        }
    }
  
    public void ResetFallParametr()
    {
        acctualyTimeFallNew = 0f;
    }

    public void CalculateFall(Vector3 direction)
    {
        CalculateJumpVarble(direction);
    }

    public void JumpUpPhysics(Vector3 direction, float rangePoint) // musi byc gdy kilkasz przycisk jump
    {
        float speed = CalculateSpeed(rangePoint);
      
        if (isSecondJumpIsAlwaseShort && isUesedDoubleJumpNew)
        {
            CalculateVelocitySecondJump(direction, speed);
        }
        else
        {
            CalculateVelocityJumpNew(direction, speed);
        }
    }

    public void FallPhysics(Vector3 direction, float rangePoint)
    {
        float speed = CalculateSpeed(rangePoint);
        CalculateVelocityForFallNew(direction, speed);
    }
    public void InicializationJump()
    {
        IsOnGroundReset();
        ResetFallParametr(); // acctualyTimeFallNew = 0f;
        ResetSelectJumpBeginningPosition();
        ResetCalculateJumpVarble();
    }

    private float CalculateFallVelocity()
    {
        float time = Time.deltaTime / maxFallDuration;
        fallStartPosition = fallCurve.Evaluate(fallPositionInCurve); // 0 jest wyzej
        fallPositionInCurve += time;
        fallDirectionPosition = fallCurve.Evaluate(fallPositionInCurve);
        fallCurveVelocity = (fallDirectionPosition - fallStartPosition) / Time.deltaTime;
        return fallCurveVelocity;
    }
    private float CalculateJumpVelocity()
    {
        float time = Time.deltaTime / maxJumpDuration;
        jumpStartPosition = jumpCurve.Evaluate(jumpPositionInCurve); // 0 jest nizej
        jumpPositionInCurve += time;
        jumpDirectionPosition = jumpCurve.Evaluate(jumpPositionInCurve);
        jumpCurveVelocity = (jumpDirectionPosition - jumpStartPosition) / Time.deltaTime;
        return jumpCurveVelocity;
    }
    private float CalculateSecondJumpVelocity()
    {
        float time = Time.deltaTime / maxSecondJumpDuration;
        secondJumpStartPosition = secondJumpCurve.Evaluate(secondJumpPositionInCurve);
        secondJumpPositionInCurve += time;
        secondJumpDirectionPosition = secondJumpCurve.Evaluate(secondJumpPositionInCurve);
        secondJumpCurveVelocity = (secondJumpDirectionPosition - secondJumpStartPosition) / Time.deltaTime;
        return secondJumpCurveVelocity;
    }
    private void ResetSelectJumpBeginningPosition()
    {
        isSelectJBP = true;
    }
    private void ResetIsBottomWaUp() //to przy podwojnym skoku i przy upadku
    {
        isBottomWasUpNew = false;
    }

    private bool isOnGround()
    {
        return jumpRayCast.RayCastCalculate();
    }

    private float CalculateSpeed(float rangePoint)
    {
        return rangePoint * 2f;
    }

    private void ResetCalculateJumpVarble()
    {
        isCalculateJumpVarble = true;
    }

    private void VelocityFallNew(Vector3 direction)
    {
        MVData.rigidbody.velocity = new Vector3(direction.x, veolcityEndCurve, direction.z);
    }

    private void CalculateVelocityJumpNew(Vector3 direction, float speed)
    {
        MVData.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);       
        direction = CalculateVelocity(direction, speed);
        MVData.rigidbody.velocity = new Vector3(direction.x, CalculateJumpVelocity(), direction.z);     
    }
 
    private void CalculateVelocitySecondJump(Vector3 direction, float speed)
    {  
        MVData.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);  
        direction = CalculateVelocity(direction, speed);
        MVData.rigidbody.velocity = new Vector3(direction.x, CalculateSecondJumpVelocity(), direction.z); //yPosition
    }

    private void CalculateVelocityForFallNew(Vector3 direction, float speed)
    {
        MVData.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration); // usuwa grawitacje
        direction = CalculateVelocity(direction, speed);
        switch (fallStep)
        {  
            case 1:
                if (fallPositionInCurve >= 1f)
                {
                    fallStep = 2;                
                }
                MVData.rigidbody.velocity = new Vector3(direction.x, CalculateFallVelocity(), direction.z);    
                break;
            case 2:             
                veolcityEndCurve = (fallCurve.Evaluate(1) - fallCurve.Evaluate(1 - Time.deltaTime / maxFallDuration))/ Time.deltaTime; /// 
                VelocityFallNew(direction);
                fallStep = 3;
                break;
            case 3:
                VelocityFallNew(direction);
                break;
        }
    }

    private Vector3 CalculateVelocity(Vector3 direction, float speed)
    {
        var target = InputManager.target.transform.position - MVData.transform.position;
        target *= speed;
        target = new Vector3(target.x, 0, target.z);
        direction = target;
        return direction;
    }

    private void ResetIsUesedDoubleJumpNew()
    {
        isUesedDoubleJumpNew = false;
    }

    private void IsOnGroundReset()
    {
        currentlyMinJumpTime = 0f;
        fallStep = 1;
        ResetSelectJumpBeginningPosition();
        ResetCalculateJumpVarble();
        ResetIsUesedDoubleJumpNew();

        ResetIsBottomWaUp();
        ResetCalculateFallVelocity();///
        ResetCalculateJumpVelocity(); //
        ResetCalculateSecondJumpVelocity(); //
    }
    private void ResetCalculateFallVelocity()
    {
        fallStartPosition = 0f;
        fallDirectionPosition = 0f;
        fallPositionInCurve = 0f;
        fallCurveVelocity = 0f;
    }
    private void ResetCalculateJumpVelocity()
    {
        jumpStartPosition = 0f;
        jumpDirectionPosition = 0f;
        jumpPositionInCurve = 0f;
        jumpCurveVelocity = 0f;
    }
    private void ResetCalculateSecondJumpVelocity()
    {
        secondJumpStartPosition = 0f;
        secondJumpDirectionPosition = 0f;
        secondJumpPositionInCurve = 0f;
        secondJumpCurveVelocity = 0f;
    }

    private void CalculateJumpVarble(Vector3 direction)
    {
        if (isCalculateJumpVarble)
        {
            finalJumpPosition = direction.y;
            MaxJumpHight = jumpCurve.Evaluate(1) + jumpBeginningPosition;
            if (finalJumpPosition > MaxJumpHight)
            {
                MaxJumpHight = finalJumpPosition;
            }
            isCalculateJumpVarble = false;
        }
    }
 
    public void setAnimatorParameters()
    {
        if (MVData.useAnimatorIsMovingParameter)
            MVData.animator.SetBool("isMoving", MVData.rigidbody.velocity.magnitude > 0.3f);
        if (MVData.useAnimatorSpeedParameter)
            MVData.animator.SetFloat("speed", MVData.rigidbody.velocity.magnitude);
        if (MVData.useAnimatorVelocityParameter)
        {
            MVData.animator.SetFloat("velocityX", MVData.rigidbody.velocity.x);
            MVData.animator.SetFloat("velocityY", MVData.rigidbody.velocity.y);
            MVData.animator.SetFloat("velocityX", MVData.rigidbody.velocity.z);
        }
    }

    [System.Serializable]
    public class Data
    {
        public Rigidbody rigidbody;
        public Animator animator;
        public Transform transform;
        public GameObject gameObject;
        public Vector3 LastVelocity;
        public bool useAnimatorIsMovingParameter;
        public bool useAnimatorSpeedParameter;
        public bool useAnimatorVelocityParameter;

        public Data(GameObject gameObject, bool useVelocityParam = false, bool useSpeedParam = true, bool useIsMovingParam = true)
        {
            this.gameObject = gameObject;
            rigidbody = gameObject.GetComponent<Rigidbody>();
            animator = gameObject.GetComponent<Animator>();
            transform = gameObject.transform;
            useAnimatorIsMovingParameter = useIsMovingParam;
            useAnimatorSpeedParameter = useSpeedParam;
            useAnimatorVelocityParameter = useVelocityParam;
        }
    }

    /// <summary>
    ///  Testowe funkcje
    /// </summary> 
    
    private float CalculateVelocity(ref float positionInCurve, float time, AnimationCurve curve)
    {      
        float startPosition  = curve.Evaluate(positionInCurve); // 0 jest nizej
        positionInCurve += time;
        float directionPosition = jumpCurve.Evaluate(positionInCurve);
        return jumpCurveVelocity = (directionPosition - startPosition) / Time.deltaTime;
    }
}
