﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {
	public TimeLayer pTimeLayer{ set; get; }

	public LayerMask collisionMask;

	public bool isWalkOnStair = false;

	public string nowHGroundTag;
	public string nowVGroundTag;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float maxClimbAngle = 80;
	float maxDescendAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider2D col;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start() {
		col = GetComponent<BoxCollider2D> ();
		pTimeLayer = transform.GetComponentInParent<TimeLayer> ();
		CalculateRaySpacing ();
	}

	public void Move(Vector3 velocity) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}
		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.position += (velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit.collider != null) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				nowHGroundTag = hit.collider.tag;

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (TimeLayer.EqualTimeLayer (pTimeLayer, hit.collider.transform.GetComponentInParent<TimeLayer> ()) ||
					   hit.collider.CompareTag ("Ground") || hit.collider.CompareTag ("GrabableGround") ||
					   hit.collider.CompareTag ("Bound") || hit.collider.CompareTag ("Stair")) {
						float distanceToSlopeStart = 0;

							if (isWalkOnStair) {
								ClimbSlope (ref velocity, slopeAngle);
							if (collisions.descendingSlope) {
								collisions.descendingSlope = false;
								velocity = collisions.velocityOld;
							}
							if (slopeAngle != collisions.slopeAngleOld) {
								distanceToSlopeStart = hit.distance - skinWidth;
								velocity.x -= distanceToSlopeStart * directionX;
							}
							velocity.x += distanceToSlopeStart * directionX;
						}
					}
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					if (TimeLayer.EqualTimeLayer (pTimeLayer, hit.collider.transform.GetComponentInParent<TimeLayer> ()) ||
					   hit.collider.CompareTag ("Ground") || hit.collider.CompareTag ("GrabableGround") ||
						hit.collider.CompareTag ("Bound") || hit.collider.CompareTag ("Stair") || 
						hit.collider.CompareTag("PassableCollision")) {

						if (isWalkOnStair && nowHGroundTag == "PassableCollision") {
							transform.position += Vector3.up * (hit.collider.bounds.max.y - col.bounds.min.y);
							Debug.Log ("Let me Lift Up");
							break;
						}

						if (!isWalkOnStair && nowHGroundTag == "Stair") {
							break;
						}

						velocity.x = (hit.distance - skinWidth) * directionX;
						collisions.left = directionX == -1;
						collisions.right = directionX == 1;

						rayLength = hit.distance;

						if (collisions.climbingSlope && isWalkOnStair) {
							velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
						}
					}

				}
			}
			else {//hit.collider == null
				nowHGroundTag = "";
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		RaycastHit2D[][] browseHits = new RaycastHit2D[verticalRayCount][];

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			browseHits[i] = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
			for (int j = 0; j < browseHits[i].Length; j++){
				var hit = browseHits [i] [j].collider;
				if (hit != null) {
					nowVGroundTag = hit.tag;

					if (TimeLayer.EqualTimeLayer (hit.GetComponentInParent<TimeLayer> (), pTimeLayer) ||
					    hit.CompareTag ("Ground") || hit.CompareTag ("GrabableGround") ||
						hit.CompareTag ("Bound") || hit.CompareTag ("Stair") || 
						hit.CompareTag("PassableCollision")) {
						
						if (hit.CompareTag ("PassableCollision") && velocity.y > 0 && (GetComponent<Player> ().isJump)) {
							break;
						}
						if (isWalkOnStair && nowVGroundTag == "PassableCollision") {
							break;
						}
						if (!isWalkOnStair && nowVGroundTag == "Stair" && velocity.y > 0){
							break;
						}

						velocity.y = (browseHits [i] [j].distance - skinWidth) * directionY;
						rayLength = browseHits [i] [j].distance;

						if (collisions.climbingSlope) {
							velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
						}
						collisions.below = directionY == -1;
						collisions.above = directionY == 1;
					}
				} 
				else { // hit == null
					nowVGroundTag = "";
				}
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) {
				if (TimeLayer.EqualTimeLayer (hit.collider.transform.GetComponentInParent<TimeLayer>(), pTimeLayer)||
					hit.collider.CompareTag("Ground")||
					hit.collider.CompareTag("Bound")||hit.collider.CompareTag("Stair")
				) {
					if(isWalkOnStair){
						float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
						if (slopeAngle != collisions.slopeAngle) {
							velocity.x = (hit.distance - skinWidth) * directionX;
							collisions.slopeAngle = slopeAngle;
						}
					}
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		Debug.Log ("In ClimbSlope");
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						if(TimeLayer.EqualTimeLayer(pTimeLayer,hit.collider.transform.GetComponentInParent<TimeLayer>())){
							float moveDistance = Mathf.Abs(velocity.x);
							float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
							velocity.y -= descendVelocityY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
						}
					}
				}
			}
		}
	}

	void UpdateRaycastOrigins() {
		CalculateRaySpacing ();
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		col = GetComponent<BoxCollider2D> ();
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}