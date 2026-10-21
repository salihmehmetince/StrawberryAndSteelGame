using UnityEngine;
public class MimicController {
 private static readonly string[] MovementNames = { "Idle", "Attack1", "Attack2", "Death", "Hurt", "IdleOpen", "IdleTransformed", "Opening", "Transform", "Walk" };
 private readonly Transform mimicTransform; private readonly Animator mimicAnimator; private readonly SpriteRenderer mimicSpriteRenderer; private Vector2 targetPosition; private bool isWalking;
 public float MovementSpeed { get; set; } public bool IsMoving => isWalking;
 public MimicController(Transform t, Animator a, SpriteRenderer r, float speed) { mimicTransform=t; mimicAnimator=a; mimicSpriteRenderer=r; MovementSpeed=speed; PlayIdle(); }
 public void Tick(float dt) { if(!isWalking)return; Vector2 p=Vector2.MoveTowards(CurrentPosition,targetPosition,MovementSpeed*dt); SetLocalPosition(p); if(Vector2.Distance(p,targetPosition)<=.001f){SetLocalPosition(targetPosition);isWalking=false;PlayIdle();} }
 public void WalkTo(Vector2 destination) { targetPosition=destination; Vector2 d=targetPosition-CurrentPosition; if(MovementSpeed<=0f||Mathf.Abs(d.x)<=.001f){SetLocalPosition(targetPosition);PlayIdle();return;} mimicSpriteRenderer.flipX=d.x<0f;isWalking=true;PlayMovement("Walk"); }
 public void PlayIdle()=>PlayMovement("Idle"); public void PlayAttack1()=>PlayMovement("Attack1"); public void PlayAttack2()=>PlayMovement("Attack2"); public void PlayDeath()=>PlayMovement("Death"); public void PlayHurt()=>PlayMovement("Hurt"); public void PlayIdleOpen()=>PlayMovement("IdleOpen"); public void PlayIdleTransformed()=>PlayMovement("IdleTransformed"); public void PlayOpening()=>PlayMovement("Opening"); public void PlayTransform()=>PlayMovement("Transform"); public void PlayWalk()=>PlayMovement("Walk");
 public void PlayMovement(string movement) { bool valid=false; foreach(string n in MovementNames){bool selected=n==movement;mimicAnimator.SetBool("Is"+n,selected);valid|=selected;} if(!valid)Debug.LogWarning("Unknown mimic movement: "+movement,mimicTransform); }
 private Vector2 CurrentPosition=>new Vector2(mimicTransform.localPosition.x,mimicTransform.localPosition.y); private void SetLocalPosition(Vector2 p)=>mimicTransform.localPosition=new Vector3(p.x,p.y,mimicTransform.localPosition.z);
}
