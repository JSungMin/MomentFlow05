using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class Controller2D : MonoBehaviour {
	public TimeLayer pTimeLayer{ set; get; }

	public LayerMask collisionMask;

	public bool isWalkOnStair = false;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float maxClimbAngle = 80;
	float maxDescendAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider col;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start() {
		col = GetComponent<BoxCollider> ();
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
			Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector3.up * (horizontalRaySpacing * i);
			RaycastHit hit;
		
			if (Physics.Raycast (rayOrigin, Vector3.right * directionX,out hit ,rayLength ,collisionMask)) {
				float slopeAngle = Vector3.Angle (hit.normal, Vector3.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (TimeLayer.EqualTimeLayer (pTimeLayer, hit.collider.transform.GetComponentInParent<TimeLayer> ()) ||
					   hit.collider.CompareTag ("Ground") || hit.collider.CompareTag ("GrabableGround") ||
					   hit.collider.CompareTag ("Bound") || hit.collider.CompareTag ("Stair")) {
						float distanceToSlopeStart = 0;
					
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

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					if (TimeLayer.EqualTimeLayer (pTimeLayer, hit.collider.transform.GetComponentInParent<TimeLayer> ()) ||
					   hit.collider.CompareTag ("Ground") || hit.collider.CompareTag ("GrabableGround") ||
						hit.collider.CompareTag ("Bound") || hit.collider.CompareTag ("Stair") || 
						hit.collider.CompareTag("PassableCollision")) {

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
		}
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		RaycastHit[][] browseHits = new RaycastHit[verticalRayCount][];

		for (int i = 0; i < verticalRayCount; i++) {
			Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector3.right * (verticalRaySpacing * i + velocity.x);
			browseHits[i] = Physics.RaycastAll(rayOrigin, Vector3.up * directionY, rayLength, collisionMask);

			for (int j = 0; j < browseHits[i].Length; j++){
				var hit = browseHits [i] [j].collider;
				if (hit != null) {

					if (TimeLayer.EqualTimeLayer (hit.GetComponentInParent<TimeLayer> (), pTimeLayer) ||
					    hit.CompareTag ("Ground") || hit.CompareTag ("GrabableGround") ||
						hit.CompareTag ("Bound") || hit.CompareTag ("Stair") || 
						hit.CompareTag("PassableCollision")) {
						
						if (hit.CompareTag ("PassableCollision") && velocity.y > 0 && (GetComponent<Player> ().isJump)) {
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
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector3 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector3.up * velocity.y;
			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, Vector3.right * directionX ,out hit ,rayLength, collisionMask)) {
				if (TimeLayer.EqualTimeLayer (hit.collider.transform.GetComponentInParent<TimeLayer>(), pTimeLayer)||
					hit.collider.CompareTag("Ground")||
					hit.collider.CompareTag("Bound")||hit.collider.CompareTag("Stair")
				) 
				{
					if(isWalkOnStair){
						float slopeAngle = Vector3.Angle(hit.normal,Vector3.up);
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
		Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit hit;
	
		if (Physics.Raycast (rayOrigin, -Vector3.up,out hit ,Mathf.Infinity, collisionMask)) {
			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
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

		raycastOrigins.bottomLeft = new Vector3 (bounds.min.x, bounds.min.y, transform.position.z);
		raycastOrigins.bottomRight = new Vector3 (bounds.max.x, bounds.min.y, transform.position.z);
		raycastOrigins.topLeft = new Vector3 (bounds.min.x, bounds.max.y, transform.position.z);
		raycastOrigins.topRight = new Vector3 (bounds.max.x, bounds.max.y, transform.position.z);
	}

	void CalculateRaySpacing() {
		col = GetComponent<BoxCollider> ();
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector3 topLeft, topRight;
		public Vector3 bottomLeft, bottomRight;
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