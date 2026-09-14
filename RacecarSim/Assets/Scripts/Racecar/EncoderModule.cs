using UnityEngine;

/// <summary>
/// Simulates the wheel encoder.
/// </summary>
public class EncoderModule : RacecarModule
{
    #region Constants

    /// <summary>
    /// The average relative error of encoder measurements.
    /// This value is made up.
    /// </summary>
    private const float errorFactor = 0.001f;

    /// <summary>
    /// The average fixed error applied to encoder measurements.
    /// This value is made up.
    /// </summary>
    private const float errorFixed = 0.01f;

    /// <summary>
    /// The gear ratio between driven and driver gears
    /// (driven gear teeth / driver gear teeth).
    /// </summary>
    private const float gearRatio = 1.0f;

    /// <summary>
    /// The resolution of the encoder in counts per revolution.
    /// Currently unused.
    /// </summary>
    private const int encoderResolution = 2500;

    /// <summary>
    /// Wheel diameter of simulated racecar.
    /// </summary>
    private const float wheelDiameter = 2.0f;

    #endregion

    #region Public Interface

    /// <summary>
    /// Encoder speed in rotations per second.
    /// </summary>
    public float RotationsPerSecond { get; private set; } = 0.0f;

    #endregion

    /// <summary>
    /// The rigidbody of the car.
    /// </summary>
    private Rigidbody rBody;

    /// <summary>
    /// Position of the car during the previous physics update.
    /// </summary>
    private Vector3 prevPosition;

    /// <summary>
    /// Change in position since the previous physics update.
    /// </summary>
    private Vector3 DeltaPosition
    {
        get
        {
            return this.rBody.position - this.prevPosition;
        }
    }

    /// <summary>
    /// Change in encoder rotations since the previous physics update.
    /// </summary>
    private float DeltaRots
    {
        get
        {
            float displacement = DeltaPosition.magnitude;

            float circumference = Mathf.PI * wheelDiameter;

            return displacement / circumference * gearRatio;
        }
    }

    protected override void Awake()
    {
        this.rBody = this.GetComponent<Rigidbody>();
        this.prevPosition = this.rBody.position;

        base.Awake();
    }

    private void FixedUpdate()
    {
        float deltaRots = this.DeltaRots;

        float rotationsPerSecond = deltaRots / Time.fixedDeltaTime; //convert to rots/secs to match expected encoder speed units

        if (Settings.IsRealism)
        {
            rotationsPerSecond *= NormalDist.Random(1, EncoderModule.errorFactor);

            rotationsPerSecond += NormalDist.Random(0, EncoderModule.errorFixed);
        }

        this.RotationsPerSecond = rotationsPerSecond;

        this.prevPosition = this.rBody.position;
    }
}