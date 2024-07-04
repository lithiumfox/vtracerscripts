using UnityEngine;
using Rewired;

public class vtracerController : MonoBehaviour
{
    // This will need clean up
    public int playerId; // Gathers playerId from Rewired
    private Player player; // Creates player for controllers
    public GameObject playerWheel, rPaddle, lPaddle, shifterSequential; // Determines objects of sim rig
    public float maxTurnAngle = 1080f; // Hardcoded rotation of wheel cause I'm being a lil lazy here
    public GameObject avatar, rig, rightHand, wheelRightHold;     // Determine Game Objects we will get positions off. (Probably easier to do with existing calibration code down the road.)
    public Vector3 rigPosition;     // Determine Vector 3 objects for getting positions of rig to avatar
    Quaternion paddleRightShiftTargetAngle = Quaternion.Euler(0, 0, -100); // I could not figure out any better way to get these to work. Determines angles of shifter/paddles to move and then bring back once action is complete. 
    Quaternion paddleLeftShiftTargetAngle = Quaternion.Euler(0, 0, 100);
    Quaternion paddleRestRightTargetAngle = Quaternion.Euler(0, 0, 100);
    Quaternion paddleRestTargetAngle = Quaternion.Euler(0, 0, -100);
    Quaternion seqShiftUpTargetAngle = Quaternion.Euler(0, 0, -100);
    Quaternion seqShiftRestUpTargetAngle = Quaternion.Euler(0, 0, 100);
    Quaternion seqShiftDownTargetAngle = Quaternion.Euler(0, 0, 100);
    Quaternion seqShiftRestTargetAngle = Quaternion.Euler(0, 0, -100);
    public float steerRotationDamp = 0.5f;


    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId); // Gets player object
        rPaddle = GameObject.FindWithTag("RightPaddle");
        lPaddle = GameObject.FindWithTag("LeftPaddle");
        playerWheel = GameObject.FindWithTag("RigWheel"); // Following finds bones named to get objects that will be used later
        //shifterSequential = GameObject.Find("ShifterShaft");
    }

    void Start()
    {

    }

    void Update()
    {
        //float currentAngle = Mathf.Clamp((player.GetAxis("Wheel") * (-maxTurnAngle/2)), -maxTurnAngle, maxTurnAngle); //Outdated method. Less prone to failure, but less smooth
        //Need to determine if any angle changes would mess this up. Also need to change 540 to pull (maxTurnAngle / 2) to get proper variable that users can put in
        playerWheel.transform.localEulerAngles = Vector3.right * Mathf.Clamp((player.GetAxis("Wheel") * (-maxTurnAngle / 2)), -maxTurnAngle, maxTurnAngle);

        // I could probably do this in a much better way. Problem is, Rewired did not provide a better example of this in their API docs. I don't really use this currently until I can figure out FinalIK stuff.

        if (player.GetButtonDown("Reset Avatar Location"))
        {
            ResetRig();
        }
        if (player.GetButtonDown("Shift Up"))
        {
            ShiftPaddleAngle(paddleRightShiftTargetAngle, rPaddle);
        }
        if (player.GetButtonUp("Shift Up"))
        {
            ShiftPaddleAngle(paddleRestRightTargetAngle, rPaddle);
        }
        if (player.GetButtonDown("Shift Down"))
        {
            ShiftPaddleAngle(paddleLeftShiftTargetAngle, lPaddle);
        }
        if (player.GetButtonUp("Shift Down"))
        {
            ShiftPaddleAngle(paddleRestTargetAngle, lPaddle);
        }
        /*if (player.GetButtonDown("SeqShift Up"))
        {
            ShiftPaddleAngle(seqShiftUpTargetAngle, rPaddle);
        }
        if (player.GetButtonUp("SeqShift Up"))
        {
            ShiftPaddleAngle(seqShiftRestUpTargetAngle, rPaddle);
        }
        if (player.GetButtonDown("SeqShift Down"))
        {
            ShiftPaddleAngle(seqShiftDownTargetAngle, lPaddle);
        }
        if (player.GetButtonUp("SeqShift Down"))
        {
            ShiftPaddleAngle(seqShiftRestTargetAngle, lPaddle);
        }*/
    }

    public void ResetRig() // This is just a really rough form of "calibration". Personally I'd like to create a saving feature here so I don't have to reset it everytime I launch the game, and I'd prefer to have the ability to drag it where it needs to be. But that requires some reverse engineering of what's going on.
    {
        avatar = GameObject.Find("Head");
        rig = GameObject.FindWithTag("SimRig");
        wheelRightHold = GameObject.Find("WheelRightHand");

        //Get Postiion of Rig, rotate it. Position it under avatar.
        rigPosition = rig.transform.position; // Initialization of what the current "Position" is.
        rig.transform.rotation = avatar.transform.rotation; //Stop gap HMD based method of determining location of rig. Major issue is it will rotate pitch & tilt as well. But it does allow for some "fine tuning" of sorts to reduce hand clipping.
        rightHand = GameObject.Find("Hand_R"); // Determines location of right hand. May need to switch to a generic hand. Determining both hands and calibrating for top and bottom of wheel would be best to allow for scaling
        rigPosition = rigPosition + LerpByDistance(wheelRightHold.transform.position, rightHand.transform.position); //Calls for LerpByDistance using the distance of a bone located on the wheel and the right hand bone.
        rig.transform.position = rigPosition; //Sets position based on return of LerpBy Distance
    }

    public void ShiftPaddleAngle(Quaternion currentAngle, GameObject shifter)
    {
        shifter.transform.Rotate(currentAngle.x * 10, currentAngle.y * 10, currentAngle.z * 10, Space.Self); //All those amazing button presses just call this but with a specific angle. At least the math is simple enough.
    }

    //Math for determining Rig to Avatar Position
    public Vector3 LerpByDistance(Vector3 A, Vector3 B)
    {
        Vector3 P = (B - A); // This feels like this could break if it was inversed somehow in world space. Curious how I could test that. 
        return P;
    }

}