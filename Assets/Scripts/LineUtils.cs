using System;
using UnityEngine;

public static class LineUtils
{
  public static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
  {
    float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

    if (tmp == 0)
    {
      // No solution!
      found = false;
      return Vector2.zero;
    }

    float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

    found = true;

    return new Vector2(
        B1.x + (B2.x - B1.x) * mu,
        B1.y + (B2.y - B1.y) * mu
    );
  }

  public static float Cross(this Vector2 v1, Vector2 v2)
  {
    return v1.x * v2.y - v1.y * v2.x;
  }
  public static bool CrossProductIntersectTest(LineSegment l1, LineSegment l2)
  {
    /**
    These two conditions can be tested for using the notion of a scalar cross product (formulas below).

      is true if and only if the scalar cross products CA→×CD and CB→×CD have opposite signs.
      is true if and only if the scalar cross products AC→×AB and AD→×AB have opposite signs.
     */

    Vector2 A = l1.StartPoint;
    Vector2 B = l1.EndPoint;
    Vector2 C = l2.StartPoint;
    Vector2 D = l2.EndPoint;

    Vector2 CA = A - C;
    Vector2 CD = D - C;
    Vector2 CB = B - C;

    float caXcd = CA.Cross(CD);
    float cbXcd = CB.Cross(CD);
    bool abDoOppose = false;
    if (Mathf.Sign(caXcd) != Mathf.Sign(cbXcd))
    {
      abDoOppose = true;
    }
    else
    {
      return false;
    }

    Vector2 AC = C - A;
    Vector2 AB = B - A;
    Vector2 AD = D - A;

    float acXab = AC.Cross(AB);
    float adXab = AD.Cross(AB);
    bool cdDoOppose = false;
    if (Mathf.Sign(acXab) != Mathf.Sign(adXab))
    {
      cdDoOppose = true;
    }

    return abDoOppose && cdDoOppose;

  }
  public static bool DoLinesIntersect(LineSegment l1, LineSegment l2)
  {
    Vector2 a = l1.EndPoint - l1.StartPoint;
    Vector2 b = l2.StartPoint - l2.EndPoint;
    Vector2 c = l1.StartPoint - l2.StartPoint;

    float alphaNumerator = b.y * c.x - b.x * c.y;
    float alphaDenominator = a.y * b.x - a.x * b.y;
    float betaNumerator = a.x * c.y - a.y * c.x;
    float betaDenominator = alphaDenominator; /*2013/07/05, fix by Deniz*/

    bool doIntersect = true;

    if (alphaDenominator == 0 || betaDenominator == 0)
    {
      doIntersect = false;
    }
    else
    {

      if (alphaDenominator > 0)
      {
        if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
        {
          doIntersect = false;
        }
      }
      else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
      {
        doIntersect = false;
      }

      if (doIntersect && betaDenominator > 0)
      {
        if (betaNumerator < 0 || betaNumerator > betaDenominator)
        {
          doIntersect = false;
        }
      }
      else if (betaNumerator > 0 || betaNumerator < betaDenominator)
      {
        doIntersect = false;
      }
    }

    return doIntersect;
  }
}