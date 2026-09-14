using UnityEngine;

/// <summary>
/// Simulates the return of get_encoder_speed.
/// </summary>
public class EncoderModule : RacecarModule
{
    #region Constants

    /// <summary>
    /// The average relative error of encoder velocity measurements.
    /// This value is made up.
    /// </summary>
    private const float velocityErrorFactor = 0.001f;

    /// <summary>
    /// The average fixed error applied to encoder velocity measurements.
    /// This value is made up.
    /// </summary>
    private const float velocityErrorFixed = 0.05f;

    #endregion

    #region Public Interface

    /// <summary>
    /// The signed forward velocity of the car in meters/second.
    /// Positive = moving forward relative to the car's transform.
    /// Negative = moving backward relative to the car's transform.
    /// </summary>
    public float SignedVelocity
    {
        get
        {
            if (!this.signedVelocity.HasValue)
            {
                Vector2 velocityXZ = new Vector2(
                    this.rBody.velocity.x,
                    this.rBody.velocity.z
                );

                float magnitude = velocityXZ.magnitude;

                float sign = Mathf.Sign(
                    Vector3.Dot(
                        this.rBody.velocity,
                        this.transform.forward
                    )
                );

                float velocity = sign * magnitude;

                if (Settings.IsRealism)
                {
                    velocity *= NormalDist.Random(1, EncoderModule.velocityErrorFactor);
                    velocity += NormalDist.Random(0, EncoderModule.velocityErrorFixed);
                }

                this.signedVelocity = velocity;
            }

            return this.signedVelocity.Value;
        }
    }

    #endregion

    /// <summary>
    /// The rigidbody of the car.
    /// </summary>
    private Rigidbody rBody;

    /// <summary>
    /// Private member for the SignedVelocity accessor.
    /// </summary>
    private float? signedVelocity = null;

    protected override void Awake()
    {
        this.rBody = this.GetComponent<Rigidbody>();

        base.Awake();
    }

    private void LateUpdate()
    {
        this.signedVelocity = null;
    }
}