﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mathx
{
	public struct Quat
	{
		public double x;
		public double y;
		public double z;
		public double w;

		public double this[int index]
		{
			get
			{
				if (index == 0) return x;
				else if (index == 1) return y;
				else if (index == 2) return z;
				else if (index == 3) return w;
				else throw (new Exception("The index is out of range!"));
			}
			set
			{
				if (index == 0) x = value;
				else if (index == 1) y = value;
				else if (index == 2) z = value;
				else if (index == 3) w = value;
				else throw (new Exception("The index is out of range!"));
			}
		}
		public bool isNaN { get { return double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z) || double.IsNaN(w); } }
		public double sqrMagnitude { get { return x * x + y * y + z * z + w * w; } }
		public double magnitude { get { return Math.Sqrt(x * x + y * y + z * z + w * w); } }

		public Quat normalized
		{
			get
			{
				double mag = Math.Sqrt(x * x + y * y + z * z + w * w);
				if (mag > 0) return new Quat(x / mag, y / mag, z / mag, w / mag);
				return new Quat();
			}
		}

		public Quat(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public string ToString(string format)
		{
			return "(" + x.ToString(format) + ", " + y.ToString(format) + ", " + z.ToString(format) + ", " + w.ToString(format) + ")";
		}
		public override string ToString()
		{
			return ToString("");
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool ValueEquals(Quat v)
		{
			return this == v;
		}


		public static explicit operator Quat(Vec3 v)
		{
			return new Quat(v.x, v.y, v.z, 0);
		}
		public static explicit operator Quat(Vec4 v)
		{
			return new Quat(v.x, v.y, v.z, v.w);
		}

		public static bool operator ==(Quat lhs, Quat rhs)
		{
			bool bx = Math.Abs(lhs.x - rhs.x) <= MathX.accuracy;
			bool by = Math.Abs(lhs.y - rhs.y) <= MathX.accuracy;
			bool bz = Math.Abs(lhs.z - rhs.z) <= MathX.accuracy;
			bool bw = Math.Abs(lhs.w - rhs.w) <= MathX.accuracy;
			return bx && by && bz && bw;
		}
		public static bool operator !=(Quat lhs, Quat rhs)
		{
			bool bx = Math.Abs(lhs.x - rhs.x) > MathX.accuracy;
			bool by = Math.Abs(lhs.y - rhs.y) > MathX.accuracy;
			bool bz = Math.Abs(lhs.z - rhs.z) > MathX.accuracy;
			bool bw = Math.Abs(lhs.w - rhs.w) > MathX.accuracy;
			return bx || by || bz || bw;
		}

		/// <summary>
		/// ~Q = (-x, -y, -z, w)
		/// </summary>
		public static Quat operator ~(Quat q)
		{
			return new Quat(-q.x, -q.y, -q.z, q.w);
		}

		/// <summary>
		/// Q1 * Q2 = (w1 * V2 + w2 * V1 + V1 x V2, w1 * w2 - V1 * V2)
		/// </summary>
		public static Quat operator *(Quat lhs, Quat rhs)
		{
			double x1 = lhs.x, y1 = lhs.y, z1 = lhs.z, w1 = lhs.w;
			double x2 = rhs.x, y2 = rhs.y, z2 = rhs.z, w2 = rhs.w;
			double x = w1 * x2 + x1 * w2 + y1 * z2 - z1 * y2;
			double y = w1 * y2 - x1 * z2 + y1 * w2 + z1 * x2;
			double z = w1 * z2 + z1 * w2 - y1 * x2 + x1 * y2;
			double w = w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2;
			return new Quat(x, y, z, w);
		}

		/// <summary>
		/// Q * V = Q * Qv * ~Q
		/// </summary>
		public static Vec3 operator *(Quat lhs, Vec3 rhs)
		{
			double x1 = lhs.x, y1 = lhs.y, z1 = lhs.z, w1 = lhs.w;
			double x2 = rhs.x, y2 = rhs.y, z2 = rhs.z;
			double nx = w1 * x2 + y1 * z2 - z1 * y2;
			double ny = w1 * y2 - x1 * z2 + z1 * x2;
			double nz = w1 * z2 + x1 * y2 - y1 * x2;
			double nw = x1 * x2 + y1 * y2 + z1 * z2;
			double vx = nw * x1 + nx * w1 - ny * z1 + nz * y1;
			double vy = nw * y1 + nx * z1 + ny * w1 - nz * x1;
			double vz = nw * z1 - nx * y1 + ny * x1 + nz * w1;
			return new Vec3(vx, vy, vz);
		}

		public static double Angle(Quat lhs, Quat rhs)
		{
			double x1 = lhs.x, y1 = lhs.y, z1 = lhs.z, w1 = lhs.w;
			double x2 = rhs.x, y2 = rhs.y, z2 = rhs.z, w2 = rhs.w;
			double dot = x1 * x2 + y1 * y2 + z1 * z2 + w1 * w2;
			double m1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1 + w1 * w1);
			double m2 = Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2 + w2 * w2);
			if (m1 == 0 || m2 == 0) return 0;
			double cos = dot / m1 / m2;
			double angle = 2.0 * Math.Acos(cos < -1 ? -1 : cos > 1 ? 1 : cos);
			if (angle > MathX.PI) angle = MathX.DoublePI - angle;
			return angle;
		}

		public static Vec3 ToEuler(Quat quat)
		{
			double qx = quat.x, qy = quat.y, qz = quat.z, qw = quat.w;
			double test = qx * qy + qz * qw;
			double vx, vy, vz;
			if (test >= 0.5)
			{
				vx = MathX.HalfPI;
				vy = 2.0 * Math.Atan2(qy, qw);
				vz = 0;
			}
			else if (test <= -0.5)
			{
				vx = -MathX.HalfPI;
				vy = -2.0 * Math.Atan2(qy, qw);
				vz = 0;
			}
			else
			{
				vx = Math.Atan2(2.0 * (qy * qw - qx * qz), 1.0 - 2.0 * (qy * qy + qz * qz));
				vy = Math.Asin(2.0 * test);
				vz = Math.Atan2(2.0 * (qx * qw - qy * qz), 1.0 - 2.0 * (qx * qx + qz * qz));
			}
			return new Vec3(vx, vy, vz);
		}

		public static Quat FromEuler(Vec3 euler)
		{
			double hx = euler.x / 2.0;
			double hy = euler.y / 2.0;
			double hz = euler.z / 2.0;
			double c1 = Math.Cos(hx);
			double c2 = Math.Cos(hy);
			double c3 = Math.Cos(hz);
			double s1 = Math.Sin(hx);
			double s2 = Math.Sin(hy);
			double s3 = Math.Sin(hz);
			double qx = s1 * c2 * c3 + c1 * s2 * s3;
			double qy = c1 * s2 * c3 + s1 * c2 * s3;
			double qz = c1 * c2 * s3 - s1 * s2 * c3;
			double qw = c1 * c2 * c3 - s1 * s2 * s3;
			return new Quat(qx, qy, qz, qw);
		}

		public static Quat AxisAngle(Vec3 axis, double angle)
		{
			double ax = axis.x, ay = axis.y, az = axis.z;
			double mag = Math.Sqrt(ax * ax + ay * ay + az * az);
			if (mag == 0) return identity;
			angle /= 2.0;
			double SdM = Math.Sin(angle) / mag;
			double qx = ax * SdM;
			double qy = ay * SdM;
			double qz = az * SdM;
			double qw = Math.Cos(angle);
			return new Quat(qx, qy, qz, qw);
		}

		public static Quat zero { get { return new Quat(); } }
		public static Quat identity { get { return new Quat(0, 0, 0, 1); } }
	}
}