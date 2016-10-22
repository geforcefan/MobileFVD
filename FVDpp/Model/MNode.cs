using System;
using GlmNet;

namespace FVD.Model
{
	public struct MNode
	{
		public vec3 Pos;
		public vec3 Dir;
		public vec3 Lat;
		public vec3 Norm;

		public float Roll;
		public float Velocity;
		public float Energy;

		public float ForceNormal;
		public float ForceLateral;
		public float SmoothNormal;
		public float SmoothLateral;

		public float DistFromLast;
		public float HeartDistFromLast;
		public float AngleFromLast;
		public float TrackAngleFromLast;
		public float DirFromLast;
		public float PitchFromLast;
		public float YawFromLast;

		public float RollSpeed;
		public float SmoothSpeed;

		public float TotalLength;
		public float TotalHeartLength;

		public MNode(MNode mnode)
		{
			Pos = new vec3(mnode.Pos);
			Dir = new vec3(mnode.Dir);
			Lat = new vec3(mnode.Lat);
			Norm = new vec3(mnode.Norm);

			Roll = mnode.Roll;
			Velocity = mnode.Velocity;
			Energy = mnode.Energy;

			ForceNormal = mnode.ForceNormal;
			ForceLateral = mnode.ForceLateral;
			SmoothNormal = mnode.SmoothNormal;
			SmoothLateral = mnode.SmoothLateral;

			DistFromLast = mnode.DistFromLast;
			HeartDistFromLast = mnode.HeartDistFromLast;
			AngleFromLast = mnode.AngleFromLast;
			TrackAngleFromLast = mnode.TrackAngleFromLast;
			DirFromLast = mnode.DirFromLast;
			PitchFromLast = mnode.PitchFromLast;
			YawFromLast = mnode.YawFromLast;

			RollSpeed = mnode.RollSpeed;
			SmoothSpeed = mnode.SmoothSpeed;

			TotalLength = mnode.TotalLength;
			TotalHeartLength = mnode.TotalHeartLength;
		}

		public MNode(vec3 aPos, 
		             vec3 aDir, 
		             float aRoll, 
		             float aVelocity, 
		             float aNormalForce, 
		             float aLateralForce)
		{

			Pos = new vec3(0.0f);
			Dir = new vec3(0.0f);
			Lat = new vec3(0.0f);
			Norm = new vec3(0.0f);

			Roll = 0.0f;
			Velocity = 0.0f;
			Energy = 0.0f;

			ForceNormal = 0.0f;
			ForceLateral = 0.0f;
			SmoothNormal = 0.0f;
			SmoothLateral = 0.0f;

			DistFromLast = 0.0f;
			HeartDistFromLast = 0.0f;
			AngleFromLast = 0.0f;
			TrackAngleFromLast = 0.0f;
			DirFromLast = 0.0f;
			PitchFromLast = 0.0f;
			YawFromLast = 0.0f;

			RollSpeed = 0.0f;
			SmoothSpeed = 0.0f;

			TotalLength = 0.0f;
			TotalHeartLength = 0.0f;

			Pos = aPos;
			Dir = glm.normalize(aDir);
			Roll = aRoll;

			Velocity = aVelocity;

			ForceNormal = aNormalForce;
			ForceLateral = aLateralForce;


			if (Dir.y == 1.0f) 
			{
				Lat = new vec3(glm.angleAxis(glm.radians(aRoll), new vec3(0.0f, -1.0f, 0.0f)) * new vec4(1.0f, 0.0f, 0.0f, 0.0f));
			}
			else
			{
				Lat = new vec3(-Dir.z, 0.0f, Dir.x);
			}

			Lat.y = glm.tan(Roll * (float)Math.PI / 180.0f) * glm.sqrt(Lat.x * Lat.x + Lat.z * Lat.z);
			Lat = glm.normalize(Lat);
		}

		public void setRoll(float Roll)
		{
			Lat = glm.normalize(glm.angleAxis(glm.radians(-Roll), Dir) * Lat);
			updateRoll();
		}

		public void changePitch(float angle, bool inverted)
		{
			vec3 rotateAround;
			//lenAssert(fabs(vLat.y) < 1.9f);
			rotateAround = glm.normalize(glm.cross(new vec3(0.0f, Norm.y, 0.0f), Dir));
			if (inverted)
			{
				rotateAround *= -1.0f;
			}
			Dir = glm.normalize(glm.angleAxis(glm.radians(angle), rotateAround) * Dir);
			Lat = glm.normalize(glm.angleAxis(glm.radians(angle), rotateAround) * Lat);
			updateNorm();
		}

		public void changeYaw(float angle)
		{
			Dir = glm.normalize(glm.angleAxis(glm.radians(angle), new vec3(0.0f, 1.0f, 0.0f)) * Dir);
			Lat = glm.normalize(glm.angleAxis(glm.radians(angle), new vec3(0.0f, 1.0f, 0.0f)) * Lat);
			updateNorm();
		}

		public void calcSmoothForces()
		{
			vec3 forceVec;
			float temp = glm.cos(glm.abs(getPitch()) * (float)Math.PI / 180.9f);
			if (glm.abs(AngleFromLast) < float.Epsilon)
			{
				forceVec = new vec3(0.0f, 1.0f, 0.0f);
			}
			else {
				float normalDAngle = (float)Math.PI / 180.0f * (-PitchFromLast * glm.cos(Roll * (float)Math.PI / 180.0f) - temp * YawFromLast * glm.sin(Roll * (float)Math.PI / 180.0f));
				float lateralDAngle = (float)Math.PI / 180.0f * (PitchFromLast * glm.sin(Roll * (float)Math.PI / 180.0f) - temp * YawFromLast * glm.cos(Roll * (float)Math.PI / 180.0f));
				forceVec = new vec3(0.0f, 1.0f, 0.0f) + lateralDAngle * Velocity * FVD.Core.Misc.F_HZ / FVD.Core.Misc.F_G * Lat + normalDAngle * HeartDistFromLast * FVD.Core.Misc.F_HZ * FVD.Core.Misc.F_HZ / FVD.Core.Misc.F_G * Norm;
			}
			SmoothNormal = -glm.dot(forceVec, glm.normalize(Norm)) - ForceNormal;
			SmoothLateral = -glm.dot(forceVec, glm.normalize(Lat)) - ForceLateral;
		}

		public void updateRoll()
		{
			updateNorm();
			Roll = glm.atan(Lat.y, -Norm.y) * 180.0f / (float)Math.PI;
		}

		public void updateNorm()
		{
			Norm = glm.cross(Dir, Lat);
		}

		public float getPitchChange()
		{
			return PitchFromLast * FVD.Core.Misc.F_HZ;
		}

		public float getYawChange()
		{
			return YawFromLast * FVD.Core.Misc.F_HZ;
		}

		public vec3 getLatHeart(float Heart)
		{
			float estimated;
			float estDistFromLast = 0.7f * HeartDistFromLast + 0.3f * DistFromLast;
			if (AngleFromLast < 0.001f)
			{
				estimated = HeartDistFromLast;
			}
			else {
				estimated = Velocity / Core.Misc.F_HZ;
			}
			float fRollSpeedPerMeter = estDistFromLast > 0.0f ? (RollSpeed + SmoothSpeed) / FVD.Core.Misc.F_HZ / estimated : 0.0f;
			return glm.normalize(glm.normalize(Lat) - glm.normalize(Dir) * (fRollSpeedPerMeter * (float)Math.PI * Heart / 180.0f));
		}

		public vec3 getDirHeart(float Heart)
		{
			float estimated;
			if (AngleFromLast < 0.001f)
			{
				estimated = HeartDistFromLast;
			}
			else {
				estimated = Velocity / FVD.Core.Misc.F_HZ;
			}

			float fRollSpeedPerMeter = HeartDistFromLast > 0.0f ? (RollSpeed + SmoothSpeed) / FVD.Core.Misc.F_HZ / estimated : 0.0f;
			if (float.IsNaN(fRollSpeedPerMeter))
				fRollSpeedPerMeter = 0.0f;

			return glm.normalize(Dir + Lat * (fRollSpeedPerMeter * (float)Math.PI * Heart / 180.0f));
		}

		public vec3 getPosHeart(float Heart)
		{
			return Pos + Heart * Norm;
		}

		public vec3 getRelPos(float y, float x, float z = 0.0f)
		{
			return Pos - y * Norm + x * getLatHeart(-y) + z * getDirHeart(-y);
		}

		public float getPitch()
		{
			return glm.atan(Dir.y, glm.sqrt(Dir.x * Dir.x + Dir.z * Dir.z)) * 180.0f / (float)Math.PI;
		}

		public float getDirection()
		{
			return glm.atan(-Dir.x, -Dir.z) * 180.0f / (float)Math.PI;
		}

		public float Flexion
		{
			get
			{
				return getFlexion();
			}
		}

		public float getFlexion()
		{
			return DistFromLast <= 0.0 ? 0.0f : TrackAngleFromLast / DistFromLast;
		}

		public override string ToString()
		{
			return string.Format("[MNode: [Pos: x {0}, y {1}, z {2}], [RollSpeed: {3}] ]", Pos.x, Pos.y, Pos.z, RollSpeed);
		}
	}
}

