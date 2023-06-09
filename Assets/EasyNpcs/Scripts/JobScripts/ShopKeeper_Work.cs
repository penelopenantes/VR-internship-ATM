using UnityEngine;
using UnityEngine.Animations.Rigging;
using AI_Package;

public class ShopKeeper_Work : Work
{
    public RigBuilder rigBuilder;
    public TwoBoneIKConstraint right;
    public TwoBoneIKConstraint left;

    public GameObject rightPlacement;
    public GameObject leftPlacement;
    public Transform lookDirection;

    Rotate rotate;

    protected virtual void OnEnable()
    {
        rotate = gameObject.AddComponent<Rotate>();
        rotate.RotateTo(lookDirection);

        right.data.target = rightPlacement.transform;
        left.data.target = leftPlacement.transform;
        rigBuilder.Build();
    }

    protected virtual void OnDisable()
    {
        right.data.target = null;
        left.data.target = null;
        rigBuilder.Build();

        Destroy(rotate);
    }
}
