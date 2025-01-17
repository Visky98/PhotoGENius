/*
PhotoGENius : photorealistic images generation.
Copyright (C) 2022  Lamorte Teresa, Salteri Francesca, Zanetti Martino

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

namespace PGENLib
{

    /// <summary>
    /// Interface for a generic 3D shape: the method RayIntersection is implemented in different concrete shapes.
    /// </summary>
    public abstract class Shape
    {
        public Transformation Transf;
        public Material Material;
        
        /// <summary>
        /// Compute the intersection between a ray and a shape
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual HitRecord? RayIntersection(Ray ray)
        {
            throw new NotImplementedException("Method Shape.RayIntersection is abstract and cannot be called");
        }

        /// <summary>
        /// Determine whether a ray hits the shape or not
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool QuickRayIntersection(Ray ray)
        {
            throw new NotImplementedException("Method Shape.QuickRayIntersection is abstract and cannot be called");
        }
    }
    
    //==============================================================================================================
    //Sphere
    //==============================================================================================================
    /// <summary>
    /// A 3D unit sphere centered on the axes origin.
    /// </summary>
    public class Sphere : Shape
    {
        public Sphere()
        {
            Transf = new Transformation();
            Material = new Material();
        }
        public Sphere(Transformation transf, Material material)
        {
            Transf = transf;
            Material = material;
        }
        
        public Sphere(Material material)
        {
            Transf = new Transformation();
            Material = material;
        }

        public Sphere(Transformation transformation)
        {
            Transf = transformation;
            Material = new Material();
            
        }

        /// <summary>
        /// Compute the intersection between a ray and the shape.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns>Returns a HitRecord, or Null if the ray doesn't hit the sphere.</returns>
        public override HitRecord? RayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(Transf.Inverse());
            var originVec = invRay.Origin.PointToVec();
            //Calculate the coefficients for the intersection equation and solve the equation
            var a = Vec.SquaredNorm(invRay.Dir);
            var b = 2.0f * Vec.DotProd(originVec, invRay.Dir);
            var c = Vec.SquaredNorm(originVec) - 1;
            var delta = b * b - 4.0f * a * c;
            
            if (delta <= 0)
            {
                return null;
            }
            var sqrtDelta = (float) Math.Sqrt(delta);
            var tmin = (-b - sqrtDelta) / (2.0f * a);
            var tmax = (-b + sqrtDelta) / (2.0f * a);
            //Choose the correct solution
            float tFirstHit;
            if (tmin > invRay.Tmin & tmin < invRay.Tmax)
            {
                tFirstHit = tmin;
            }
            else if (tmax > invRay.Tmin & tmax < invRay.Tmax)
            {
                tFirstHit = tmax;
            }
            else
            {
                return null;
            }
            //Calculate the hit point
            var hitPoint = invRay.At(tFirstHit);
            var hit = new HitRecord(Transf*hitPoint, Transf*SphereNormal(hitPoint, ray.Dir),
                SpherePointToUv(hitPoint), tFirstHit, ray, Material);
            return hit;
        }

        /// <summary>
        /// Determine whether a ray hits the shape or not
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool QuickRayIntersection(Ray ray)
        {
            var invRay = ray.Transform(Transf.Inverse());
            var originVec = invRay.Origin.PointToVec();
            
            //Calculate the coefficients for the intersection equation and solve the equation
            var a = Vec.SquaredNorm(invRay.Dir);
            var b = 2.0f * Vec.DotProd(originVec, invRay.Dir);
            var c = Vec.SquaredNorm(originVec) - 1.0f;
            var delta = b * b - 4.0 * a * c;
            
            //Is there any solution?
            if (delta <= 0.0f)
            {
                return false;
            }

            var sqrtDelta = (float) Math.Sqrt(delta);
            var tmin = (-b - sqrtDelta) / (2.0f * a);
            var tmax = (-b + sqrtDelta) / (2.0f * a);
            
            return ((tmin > invRay.Tmin && tmin < invRay.Tmax) || (tmax > invRay.Tmin && tmax < invRay.Tmax));
        }
        
        /// <summary>
        /// Computes the normal in the intersection point of a ray and the surface of a unit sphere.
        /// The normal has always the opposite direction with respect to a given Ray. 
        /// </summary>
        /// <param name="point">Type Point</param>
        /// <param name="dir">Type Vec</param>
        /// <returns>Normal</returns>
        public Normal SphereNormal(Point point,  Vec dir)
        {
            Normal result = new Normal(point.x, point.y, point.z);
            Normal n;
            if (Vec.DotProd(point.PointToVec(),dir) < 0.0)
            {
                n = result;
            }
            else
            {
                n = result*(-1);
            }
            return n;
        }

        /// <summary>
        /// Convert the 3D intersection point into a 2D point, of coordinates (u,v). 
        /// </summary>
        /// <param name="point"></param>
        /// <returns>Vec2d</returns>
        public Vec2d SpherePointToUv(Point point)
        {
            var u = (float)(Math.Atan2(point.y, point.x) / (2.0 * Math.PI));
            var v = (float)(Math.Acos(point.z) / Math.PI);
            if (u >= 0)
            {
                return new Vec2d(u, v);
            }
            return new Vec2d(u+1f, v);
        }
    }

    //==============================================================================================================
    //Plane
    //==============================================================================================================
    
    /// <summary>
    ///  A 2D plane that you can put in the scene. Unless transformations are applied, the z=0 plane is constructed.
    /// </summary>
    public class XyPlane : Shape
    {
        /// <summary>
        /// Constructor without any parameter.
        /// </summary>
        public XyPlane()
        {
            Transf = new Transformation();
            Material = new Material();
        }
        
        /// <summary>
        /// Constructor with parameters. 
        /// </summary>
        /// <param name="transf"></param>
        /// <param name="material"></param>
        public XyPlane(Transformation transf, Material material)
        {
            Transf = transf;
            Material = material;
        }
        
        /// <summary>
        /// Compute the intersection between a ray and the shape.
        /// Returns a HitRecord object, or Null if the ray doesn't hit the plane.
        /// </summary>
        public override HitRecord? RayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.Transf.Inverse());
            var originVec = invRay.Origin.PointToVec();

            if (Math.Abs(invRay.Dir.z) < 1E-5)
            {
                return null;
            }
            var t = - originVec.z/invRay.Dir.z;
            if (t <= invRay.Tmin || t >= invRay.Tmax)
            {
                return null;
            }
            var hitPoint = invRay.At(t);
            var planeNormal = new Normal(0.0f, 0.0f, 1.0f);
            if (invRay.Dir.z > 0)
            {
                planeNormal = planeNormal * -1.0f;
            } 
            
            var surfacePoint = new Vec2d(hitPoint.x - (float)Math.Floor(hitPoint.x),  
                hitPoint.y - (float)Math.Floor(hitPoint.y));
            var hit = new HitRecord(Transf*hitPoint, Transf*planeNormal,
                surfacePoint, t, ray, Material);
            return hit;
        }
        
        /// <summary>
        /// Determine whether a ray hits the shape or not
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool QuickRayIntersection(Ray ray)
        {
            var invRay = ray.Transform(Transf.Inverse());
            if (Math.Abs(invRay.Dir.z) < 1e-5)
            {
                return false;
            }

            var t = -invRay.Origin.z / invRay.Dir.z;
            return t > invRay.Tmin && t < invRay.Tmax;
        }
        
    }
    
    //==============================================================================================================
    //Cylinder
    //==============================================================================================================
    /// <summary>
    /// Class to represent a cylinder.
    /// </summary>
    public class Cylinder : Shape
    {
        public float R;
        public float Zmin;
        public float Zmax;
        public float Phimax;
        
        
        /// <summary>
        /// Constructor without any parameter.
        /// </summary>
        public Cylinder(float zmin = 0.0f, float zmax = 2.0f, float r=1.0f)
        {
            Phimax = (float) (2*Math.PI);
            Zmin = zmin;
            Zmax = zmax;
            Transf = new Transformation();
            Material = new Material();
        }
        
        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public Cylinder(Material material, float zmin = 0.0f, float zmax = 2.0f, float r=1.0f)
        {
            Phimax = (float) (2*Math.PI);
            Zmin = zmin;
            Zmax = zmax;
            Transf = new Transformation();
            Material = material;
        }
        
        /// <summary>
        /// Constructor with parameters. 
        /// </summary>
        /// <param name="transf"></param>
        /// <param name="material"></param>
        /// <param name="zmin"></param>
        /// <param name="zmax"></param>
        /// <param name="r"></param>
        public Cylinder(Transformation transf, Material material, float zmin = 0f, float zmax = 2f, float r = 1.0f)
        {
            Zmin = zmin;
            Zmax = zmax;
            Phimax = (float) (2*Math.PI);
            R = r;
            Transf = transf;
            Material = material;
        }

        /// <summary>
        /// Constructor with parameters. 
        /// </summary>
        /// <param name="transf"></param>
        /// <param name="material"></param>
        /// <param name="zmin"></param>
        /// <param name="zmax"></param>
        /// <param name="phimax"></param>
        /// <param name="r"></param>
        public Cylinder(Transformation transf, Material material, float zmin, float zmax, float phimax, float r = 1.0f)
        {
            Zmin = zmin;
            Zmax = zmax;
            Phimax = phimax;
            R = r;
            Transf = transf;
            Material = material;
        }
        
        /// <summary>
        /// Compute the intersection between a ray and the shape.
        /// Returns a HitRecord object, or Null if the ray doesn't hit the plane.
        /// </summary>
        public override HitRecord? RayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.Transf.Inverse());
            var originVec = invRay.Origin.PointToVec();

            //Calculate the coefficients for the intersection equation and solve the equation

            var a = invRay.Dir.x*invRay.Dir.x + invRay.Dir.y*invRay.Dir.y;
            var b = 2*(originVec.x * invRay.Dir.x + originVec.y * invRay.Dir.y);
            var c = originVec.x * originVec.x + originVec.y * originVec.y - R * R;
            var delta = b * b - 4.0f * a * c;
            
            if (delta <= 0)
            {
                return null;
            }
            
            var sqrtDelta = (float) Math.Sqrt(delta);
            var tmin = (-b - sqrtDelta) / (2.0f * a);
            var tmax = (-b + sqrtDelta) / (2.0f * a);

            if (tmin > tmax)
            {
                //Swap values
                (tmin, tmax) = (tmax, tmin); 
            }
            
            //Choose the correct solution
            float tFirstHit;
            if (tmin > invRay.Tmin & tmin < invRay.Tmax)
            {
                tFirstHit = tmin;
            }
            else if (tmax > invRay.Tmin & tmax < invRay.Tmax)
            {
                tFirstHit = tmax;
            }
            else
            {
                return null;
            }

            //Evaluate the hitpoint and phi
            var hitPoint = invRay.At(tFirstHit);
            var phi = (float)Math.Atan2(hitPoint.y, hitPoint.x);
            
            //Boundary conditions for z and phi
            if (phi < 0)
            {
                phi += (float)(2 * Math.PI);
            }
            if (hitPoint.z < Zmin || hitPoint.z > Zmax || phi > Phimax)
            {
                if (Math.Abs(tFirstHit - tmax) < 1E-5)
                {
                    return null;
                }

                tFirstHit = tmax;
                if (tFirstHit > invRay.Tmax) return null;

                hitPoint = invRay.At(tFirstHit);
                phi = (float)Math.Atan2(hitPoint.y, hitPoint.x);

                if (phi < 0)
                {
                    phi += (float) (2 * Math.PI);
                }
                if (hitPoint.z < Zmin || hitPoint.z > Zmax || phi > Phimax)
                {
                    return null;
                }
            }

            var normal = new Normal(hitPoint.x, hitPoint.y, 0f);
            var hit = new HitRecord(Transf*hitPoint, Transf*normal,
                new Vec2d(phi / Phimax, (hitPoint.z - Zmin) / (Zmax - Zmin)), tFirstHit, ray, Material);
            return hit;
        }
        
        /// <summary>
        /// Determine whether a ray hits the shape or not
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool QuickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(Transf.Inverse());
            var originVec = invRay.Origin.PointToVec();
            
            var a = invRay.Dir.x*invRay.Dir.x + invRay.Dir.y*invRay.Dir.y;
            var b = 2*(originVec.x * invRay.Dir.x + originVec.y * invRay.Dir.y);
            var c = originVec.x * originVec.x + originVec.y * originVec.y - R * R;
            var delta = b * b - 4.0f * a * c;
            
            if (delta < 0)
            {
                return false;
            }
            var sqrtDelta = (float) Math.Sqrt(delta);
            var tmin = (-b - sqrtDelta) / (2.0f * a);
            var tmax = (-b + sqrtDelta) / (2.0f * a);
            if (tmin > tmax)
            {
                //Swap values
                (tmin, tmax) = (tmax, tmin); 
            }
            
            //Choose the correct solution
            float tFirstHit;
            
            if (tmin > invRay.Tmin & tmin < invRay.Tmax)
            {
                tFirstHit = tmin;
            }
            else if (tmax > invRay.Tmin & tmax < invRay.Tmax)
            {
                tFirstHit = tmax;
            }
            else
            {
                return false;
            }

        
            //Evaluate the hitpoint
            var hitPoint = invRay.At(tFirstHit);
            //Compute phi
            var phi = (float)Math.Atan2(hitPoint.y, hitPoint.x);
            if (phi < 0)
            {
                phi += (float)(2 * Math.PI);
            }
            
            //Boundary conditions for z and phi
            if (hitPoint.z < Zmin || hitPoint.z > Zmax || phi > Phimax)
            {
                if (Math.Abs(tFirstHit - tmax) < 1E-5)
                {
                    return false;
                }

                tFirstHit = tmax;
                if (tFirstHit > invRay.Tmax) return false;

                hitPoint = invRay.At(tFirstHit);
                phi = (float)Math.Atan2(hitPoint.y, hitPoint.x);

                if (phi < 0)
                {
                    phi += (float) (2 * Math.PI);
                }
                if (hitPoint.z < Zmin || hitPoint.z > Zmax || phi > Phimax)
                {
                    return false;
                }
            }

            return true;
        }
        
    }

}