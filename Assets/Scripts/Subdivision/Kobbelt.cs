using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InterfaceUtils;

public static class Kobbelt {

    public static void Subdivision(ref List<Triangle> triangles)
    {
        // ***** Split *****

        var hit_triangles = new List<Triangle>(triangles);

        IEnumerable<Edge> edge_stack = Enumerable.Empty<Edge>();
        foreach (var ht in hit_triangles)
        {
            // On stock les arêtes
            edge_stack = edge_stack.Concat(ht.edges);

            // On supprime le triangle
            int index = triangles.FindIndex(t => t.isEqual(ht));
            triangles.RemoveAt(index);

            Vector3 center = ht.Center();

            // On le split en trois
            for (var k = 0; k < 3; k++)
            {
                Triangle t = new Triangle(
                    ht.vertices[k],
                    ht.vertices[(k + 1) % 3],
                    center
                );

                triangles.Add(t);
            }
        }

        var copy_triangles = new List<Triangle>(triangles);
        for (int i = 0; i < triangles.Count; i += 3)
        {
            for (var j = 0; j < 3; j++)
            {
                Vector3 P = triangles[i].vertices[j];

                var neighborhoods = copy_triangles
                    .Where(t => t.vertices.Contains(P))
                    .Select(t => {
                        for (int i = 0; i < 3; i++)
                            if (t.edges[i].start == P) {
                                return t.edges[i].end;
                            } else if (t.edges[i].end == P) {
                                return t.edges[i].start;
                            }
                        Debug.Assert(false);
                        return Vector3.zero;
                    })
                    .ToList();

                neighborhoods.Add(P);
                Vector3 vCenter = Interface.findCenter(neighborhoods);

                float n = neighborhoods.Count;
                Debug.Assert(n != 0);
                float alpha = (4.0f - 2.0f * Mathf.Cos(2.0f * Mathf.PI / n)) / 9.0f;

                triangles[i].vertices[j] = (1 - alpha) * P + (vCenter * alpha);
            }
        }

        // ***** Flip *****
        Flip(ref edge_stack, in copy_triangles, ref triangles);
    }

    /**
     * Flip
     */
    public static void Flip(ref IEnumerable<Edge> edge_stack, in List<Triangle> copy_triangles, ref List<Triangle> triangles)
    {
        foreach (var edge in edge_stack)
        {
            // On trouve les triangles qui partage le coté courant
            var common_edge_triangles_indices = copy_triangles
                .Select((t, i) => new { Value = t, Index = i })
                .Where(t => t.Value.hasEdge(edge))
                .Select(t => t.Index)
                .ToList();

            // On ignorer s'il n'y a pas au minimum deux triangles
            if (common_edge_triangles_indices.Count < 2) continue;

            var triangle_ABC = triangles[common_edge_triangles_indices[0]];
            var triangle_ABD = triangles[common_edge_triangles_indices[0]];

            var point_A = edge.start;
            var point_B = edge.end;

            // On parcours déjà les triangles subdivisé

            // On récupère le points qui n'est pas sur l'arête, parmi les sommets du triangle ABC (le point C)
            var point_C = triangle_ABC.GetOtherPoint(edge);

            // Pareil pour le triangle ABD (le point D)
            var point_D = triangle_ABD.GetOtherPoint(edge);

            {
                // On supprime les triangles de la liste des triangles
                int index_ABC = triangles.FindIndex(t => t.isEqual(triangle_ABC));
                triangles.RemoveAt(index_ABC);
                int index_ABD = triangles.FindIndex(t => t.isEqual(triangle_ABD));
                triangles.RemoveAt(index_ABD);

                // On crée les deux triangles en flipant l'arête
                var triangle_ACD = new Triangle(point_A, point_D, point_C);
                var triangle_BCD = new Triangle(point_B, point_C, point_D);

                // On les ajout à la liste de triangle
                triangles.Add(triangle_ACD);
                triangles.Add(triangle_BCD);
            }
        }
    }
}