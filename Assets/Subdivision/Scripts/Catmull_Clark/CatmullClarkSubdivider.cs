using System;
using System.Collections.Generic;
using System.Linq;


namespace Subdivision.Core
{
    public class CatmullClarkSubdivider : ISubdivider
    {
        public Shape Subdivide(Shape shape)
        {
            //Creation d'une nouvelle shape
            Shape subdivided = new Shape();

            //Pour chaque face, on creer un facepoint utilisant la moyen de chaque point.
            CreateFacePoints(shape);

            //Creation du Edge point, en utilisant la moyen du centre de edge et celui du segment qui relie de FacePoint.
            CreateEdgePoints(shape);

            //Creation des Vertexpoint utilisant la formule du cours
            CreateVertexPoints(shape);

            //for a triangle face (a,b,c): 
            //   (a, edge_pointab, face_pointabc, edge_pointca)
            //   (b, edge_pointbc, face_pointabc, edge_pointab)
            //   (c, edge_pointca, face_pointabc, edge_pointbc)

            //for a quad face (a,b,c,d): 
            //   (a, edge_pointab, face_pointabcd, edge_pointda)
            //   (b, edge_pointbc, face_pointabcd, edge_pointab)
            //   (c, edge_pointcd, face_pointabcd, edge_pointbc)
            //   (d, edge_pointda, face_pointabcd, edge_pointcd)
            CreateFaces(shape, subdivided);

            return subdivided;
        }

        private void CreateFacePoints(Shape shape)
        {
            //Pour chaque face, on calcule sa moyenne de toute ses point afin de obtenir les FacePoint de chauqe Face
            foreach (Face face in shape.Faces)
            {
                List<Point> points = face.AllPoints;
                face.FacePoint = new Point(Average(points));
            }
        }

        private void CreateEdgePoints(Shape shape)
        {
            //Pour obtenir le EdgePoint, on fait la moyenne entre les point de mon segment et celui qui relie au FacePoint
            List<Edge> edges = shape.AllEdges;
            foreach (Edge edge in edges)
            {
                //Quand on tombe sur un bord ou il y a un troue
                if (edge.IsOnBorderOfHole)
                {
                    Vector3 position =
                        Average(
                            edge.Points[0],
                            edge.Points[1]);

                    edge.EdgePoint = new Point(position);
                }
                else
                {
                    Vector3 position =
                        Average(
                            edge.Points[0],
                            edge.Points[1],
                            edge.Faces[0].FacePoint,
                            edge.Faces[1].FacePoint);

                    edge.EdgePoint = new Point(position);
                }
            }
        }

        private void CreateVertexPoints(Shape shape)
        {

            //Creation du vertexPoint, on modifie les point Actuelle qui est le oldPoint en suite le recreer dqui est donc  le Succesor
            List<Point> allPoints = shape.AllPoints;
            List<Edge> allEdges = shape.AllEdges;

            foreach (Point oldPoint in allPoints)
            {
                if (oldPoint.IsOnBorderOfHole)
                {
                    oldPoint.Successor = CreateVertexPointForBorderPoint(oldPoint);
                }
                else
                {
                    //Les nouveau vertex point obtenu on le retien dans le Succesor
                    oldPoint.Successor = CreateVertexPoint(allEdges, oldPoint);
                }
            }
        }

        private Point CreateVertexPoint(List<Edge> allEdges, Point oldPoint)
        {

            // On fait la moyenne des midPoint equivalent de "R" du cours 
            Vector3 avgMidEdges = Vector3.Average(oldPoint.Edges.Select(e => e.Middle));
            
            List<Face> pointFaces = oldPoint.AllFaces;
            //La moyenne des faces Point equivalent de "Q"
            Vector3 avgFacePoints = Average(pointFaces.Select(pf => pf.FacePoint));

            //Le nombre de face compter equivalent au "n" de la formule du cours
            int faceCount = pointFaces.Count;

            
            double m1 = (faceCount - 3f) / faceCount;   // n-3/n
            double m2 = 1f / faceCount;                 // 1/n
            double m3 = 2f / faceCount;                 // 2/n

            //On applique v' = n-3/n *  v     +         1/n * Q       +         2/n  *  R  
            Vector3 position = m1 * oldPoint.Position + m2 * avgFacePoints + m3 * avgMidEdges;
         
            return new Point(position);
        }

        private Point CreateVertexPointForBorderPoint(Point oldPoint)
        { 
            // calculate the average between these points (on the hole boundary) and the old coordinates (also on the hole boundary). 
            List<Vector3> positions = oldPoint.Edges.Where(e => e.IsOnBorderOfHole).Select(e => e.Middle).ToList();
            positions.Add(oldPoint.Position);

            return new Point(Vector3.Average(positions));
        }

        private void CreateFaces(Shape shape, Shape subdivided)
        {
            List<Face> faces = shape.Faces;
            List<Edge> existingEdges = new List<Edge>();
            //Pour chaque face de ma liste de toute mes face
            foreach (Face face in faces)
            {
                if (face.AllPoints.Count() == 3)
                {
                    CreateTriangleFace(existingEdges, subdivided, face);
                }
                else if (face.AllPoints.Count() == 4)
                {
                    //CreateQuadFace(existingEdges, subdivided, face);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unhandled facetype (point count={0})!", face.AllPoints.Count()));
                }
            }
            SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
        }

        private void CreateTriangleFace(List<Edge> existingEdges, Shape subdivided, Face face)
        {
            List<Point> points = face.AllPoints;
            Point a = points[0].Successor;
            Point b = points[1].Successor;
            Point c = points[2].Successor;

            //for a triangle face (a,b,c): 
            //   (a, edge_pointab, face_pointabc, edge_pointca)
            //   (b, edge_pointbc, face_pointabc, edge_pointab)
            //   (c, edge_pointca, face_pointabc, edge_pointbc)
            Point facePoint = face.FacePoint;

            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, a, face.Edges[0].EdgePoint, face.Edges[2].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, b, face.Edges[1].EdgePoint, face.Edges[0].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, c, face.Edges[2].EdgePoint, face.Edges[1].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, face.Edges[0].EdgePoint, face.Edges[1].EdgePoint, face.Edges[2].EdgePoint));

            SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
        }

        private void CreateQuadFace(List<Edge> existingEdges, Shape subdivided, Face face)
        {
            //                  0 1 2 -> 3 
            //for a quad face (a,b,c,d): 
            //   (a, edge_pointab, face_pointabcd, edge_pointda)
            //   (b, edge_pointbc, face_pointabcd, edge_pointab)
            //   (c, edge_pointcd, face_pointabcd, edge_pointbc)
            //   (d, edge_pointda, face_pointabcd, edge_pointcd)
            List<Point> points = face.AllPoints;
            Point a = points[0].Successor;
            Point b = points[1].Successor;
            Point c = points[2].Successor;
            Point d = points[3].Successor;

            Point facePoint = face.FacePoint;

            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, a, face.Edges[0].EdgePoint, facePoint, face.Edges[3].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, b, face.Edges[1].EdgePoint, facePoint, face.Edges[0].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, c, face.Edges[2].EdgePoint, facePoint, face.Edges[1].EdgePoint));
            subdivided.Faces.Add(SubdivisionUtilities.CreateFaceF(existingEdges, d, face.Edges[3].EdgePoint, facePoint, face.Edges[2].EdgePoint));

            SubdivisionUtilities.VerifyThatThereAreNoEdgeDuplicates(existingEdges);
        }

        private Vector3 Average(IEnumerable<Point> points)
        {
            return Vector3.Average(points.Select(p => p.Position));
        }

        private Vector3 Average(params Point[] points)
        {
            return Vector3.Average(points.Select(p => p.Position));
        }
    }
}