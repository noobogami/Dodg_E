using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Animate()
    {
        Animation anim = GetComponent<Animation>();
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        // create a curve to move the GameObject and assign to the clip
        Keyframe[] keys;
        keys = new Keyframe[3];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(1.0f, 1.5f);
        keys[2] = new Keyframe(2.0f, 0.0f);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        // now animate the GameObject
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
