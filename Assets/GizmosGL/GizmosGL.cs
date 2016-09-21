using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class GizmosGL : MonoBehaviour
{
    // container storing the definition of a vertex
    public class Vertex
    {
        public Vector3 position;
        public Color colour;
        public Vertex(Vector3 position, Color colour)
        {
            this.position = position;
            this.colour = colour;
        }
    }

    //Line container
    public class Line
    {
        public Vertex v0, v1;
        public Line(Vertex v0, Vertex v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
    }
    //Tri container
    public class Tri
    {
        public Vertex v0, v1, v2;
        public Tri(Vertex v0, Vertex v1, Vertex v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    public const int TOP_LEFT = 0;
    public const int BOTTOM_LEFT = 1;
    public const int BOTTOM_RIGHT = 2;
    public const int TOP_RIGHT = 3;
    public const int BACK_TOP_LEFT = 4;
    public const int BACK_BOTTOM_LEFT = 5;
    public const int BACK_BOTTOM_RIGHT = 6;
    public const int BACK_TOP_RIGHT = 7;



    //this variable is used to make gizmosgl a singleton
    public static GizmosGL instance = null;
    public Shader shader = null;
    public Line[] lines;
    public Tri[] tris;
    public int lineIndex;
    public int triIndex;
    public int maxLines = 1500;
    public int maxTris = 1500;
    public BlendMode srcBlend = BlendMode.SrcAlpha;
    public BlendMode dstBlend = BlendMode.OneMinusSrcAlpha;
    public CullMode cullMode = CullMode.Front;
    public bool zWrite = true;

    private Material material = null;

    void Awake()
    {
        // Check if instance has been created
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        //Allocate memory for our lines & triangles
        lines = new Line[maxLines];
        tris = new Tri[maxTris];

        // Create our material for drawing lines and tris
        CreateMaterial();
    }


    void CreateMaterial()
    {
        // Create our material from shader
        material = new Material(shader);
        //Turn on alpha blending
        material.SetInt("_SrcBlend", (int)srcBlend);
        material.SetInt("_DstBlend", (int)dstBlend);
        // Turn on frontface
        material.SetInt("_Cull", (int)cullMode);
        //Turn on Zwrites
        material.SetInt("_ZWrite", zWrite ? 1 : 0);
    }

    /*
     *Place all of your draw shape code here
     */
    public static void AddLine(Vector3 start, Vector3 end, Color colour, Color? endColour = null)
    {
        if (instance.lineIndex >= instance.maxLines) return;
        Vertex v0 = new Vertex(start, colour);
        Vertex v1 = new Vertex(end, endColour == null ? colour : endColour.Value);
        instance.lines[instance.lineIndex++] = new Line(v0, v1);
    }

    public static void AddTri(Vector3 vert0, Vector3 vert1, Vector3 vert2, Color color)
    {
        if (instance.triIndex >= instance.maxTris) return;
        Vertex v0 = new Vertex(vert0, color);
        Vertex v1 = new Vertex(vert1, color);
        Vertex v2 = new Vertex(vert2, color);
        instance.tris[instance.triIndex++] = new Tri(v0, v1, v2);

    }

    public static void AddCircle(Vector2 position, float radius, int segments, Color color, bool isFilled = true)
    {
        float segmentSize = (2 * Mathf.PI) / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector2 pos0 = new Vector2(Mathf.Sin(i * segmentSize) * radius,
                                        Mathf.Cos(i * segmentSize) * radius);
            Vector2 pos1 = new Vector2(Mathf.Sin((i + 1) * segmentSize) * radius,
                                        Mathf.Cos((i + 1) * segmentSize) * radius);

            AddLine(position + pos0, position + pos1, color);

            if (isFilled)
            {
                AddTri(position, position + pos0, position + pos1, color);
            }
        }


    }

    public static void AddSquare(Vector2 position, Vector2 scale, Color color, bool isFilled = true)
    {
        Vector2 vX = new Vector2(scale.x, 0);
        Vector2 vY = new Vector2(0, scale.y);


        Vector2[] corners = new Vector2[4];

        //Top left
        corners[TOP_LEFT] = position - vX + vY;
        //bottom left
        corners[BOTTOM_LEFT] = position - vX - vY;
        //bottom right
        corners[BOTTOM_RIGHT] = position + vX - vY;
        //top right
        corners[TOP_RIGHT] = position + vX + vY;

        //line from top left to bottom left
        AddLine(corners[TOP_LEFT], corners[BOTTOM_LEFT], color);
        //line from bottom left to bottom right
        AddLine(corners[BOTTOM_LEFT], corners[BOTTOM_RIGHT], color);
        //line from bottom right to top right
        AddLine(corners[BOTTOM_RIGHT], corners[TOP_RIGHT], color);
        //line from top right to top left
        AddLine(corners[TOP_RIGHT], corners[TOP_LEFT], color);


        if (isFilled)
        {
            AddTri(corners[TOP_LEFT], corners[BOTTOM_LEFT], corners[BOTTOM_RIGHT], color);
            AddTri(corners[BOTTOM_RIGHT], corners[TOP_RIGHT], corners[TOP_LEFT], color);
        }

    }

    public static void AddCube(Vector3 position, Vector3 scale, Color color, bool isFilled = true)
    {
        Vector3 vX = new Vector3(scale.x, 0, 0);
        Vector3 vY = new Vector3(0, scale.y, 0);
        Vector3 vZ = new Vector3(0, 0, scale.z);

        Vector3[] corners = new Vector3[8];

        //Top left
        corners[TOP_LEFT] = position - vX + vY + vZ;
        //bottom left
        corners[BOTTOM_LEFT] = position - vX - vY + vZ;
        //bottom right
        corners[BOTTOM_RIGHT] = position + vX - vY + vZ;
        //top right
        corners[TOP_RIGHT] = position + vX + vY + vZ;

        //back Top left
        corners[BACK_TOP_LEFT] = position - vX + vY - vZ;
        //back bottom left
        corners[BACK_BOTTOM_LEFT] = position - vX - vY - vZ;
        //back  bottom right
        corners[BACK_BOTTOM_RIGHT] = position + vX - vY - vZ;
        //back top right
        corners[BACK_TOP_RIGHT] = position + vX + vY - vZ;

        //line from top left to bottom left
        AddLine(corners[TOP_LEFT], corners[BOTTOM_LEFT], color);
        //line from bottom left to bottom right
        AddLine(corners[BOTTOM_LEFT], corners[BOTTOM_RIGHT], color);
        //line from bottom right to top right
        AddLine(corners[BOTTOM_RIGHT], corners[TOP_RIGHT], color);
        //line from top right to top left
        AddLine(corners[TOP_RIGHT], corners[TOP_LEFT], color);

        //back line from top left to bottom left
        AddLine(corners[BACK_TOP_LEFT], corners[BACK_BOTTOM_LEFT], color);
        //back line from bottom left to bottom right
        AddLine(corners[BACK_BOTTOM_LEFT], corners[BACK_BOTTOM_RIGHT], color);
        //back line from bottom right to top right
        AddLine(corners[BACK_BOTTOM_RIGHT], corners[BACK_TOP_RIGHT], color);
        //back line from top right to top left
        AddLine(corners[BACK_TOP_RIGHT], corners[BACK_TOP_LEFT], color);

        //line from top left to top left
        AddLine(corners[BACK_TOP_LEFT], corners[TOP_LEFT], color);
        //line from bottom left to bottom left
        AddLine(corners[BACK_BOTTOM_LEFT], corners[BOTTOM_LEFT], color);
        //line from bottom right to bottom right
        AddLine(corners[BACK_BOTTOM_RIGHT], corners[BOTTOM_RIGHT], color);
        //line from top right to top right
        AddLine(corners[BACK_TOP_RIGHT], corners[TOP_RIGHT], color);


        if (isFilled)
        {
            //Front face
            AddTri(corners[BOTTOM_RIGHT], corners[BOTTOM_LEFT], corners[TOP_LEFT], color);
            AddTri(corners[TOP_LEFT], corners[TOP_RIGHT], corners[BOTTOM_RIGHT], color);

            //Back face
            AddTri(corners[BACK_TOP_LEFT], corners[BACK_BOTTOM_LEFT], corners[BACK_BOTTOM_RIGHT], color);
            AddTri(corners[BACK_BOTTOM_RIGHT], corners[BACK_TOP_RIGHT], corners[BACK_TOP_LEFT], color);

            //Top face
            AddTri(corners[TOP_RIGHT], corners[BACK_TOP_LEFT], corners[BACK_TOP_RIGHT], color);
            AddTri(corners[BACK_TOP_LEFT], corners[TOP_RIGHT], corners[TOP_LEFT], color);

            //Bottom face
            AddTri(corners[BOTTOM_RIGHT], corners[BACK_BOTTOM_RIGHT], corners[BACK_BOTTOM_LEFT], color);
            AddTri(corners[BOTTOM_LEFT], corners[BOTTOM_RIGHT], corners[BACK_BOTTOM_LEFT], color);

            //LEFT face
            AddTri(corners[TOP_LEFT], corners[BOTTOM_LEFT], corners[BACK_BOTTOM_LEFT], color);
            AddTri(corners[BACK_BOTTOM_LEFT], corners[BACK_TOP_LEFT], corners[TOP_LEFT], color);

            //RIGHT face
            AddTri(corners[BACK_BOTTOM_RIGHT], corners[BOTTOM_RIGHT], corners[TOP_RIGHT], color);
            AddTri(corners[TOP_RIGHT], corners[BACK_TOP_RIGHT], corners[BACK_BOTTOM_RIGHT], color);
        }


    }

    public static void AddSphere(Vector3 position, float radius, int rows, int columns, Color color, bool isFilled = true,
                                    float longMin = 0f, float longMax = 360f, float latMin = -90f, float latMax = 90f)
    {
        // Set these values as inverse for multiplicatin later
        float inverseRadius = 1f / radius;
        float inverseColumns = 1f / columns;
        float inverseRows = 1f / rows;

        //Calculate range for sphere to be drawn(lat and long)
        float latRange = (latMax - latMin) * Mathf.Deg2Rad;
        float longRange = (longMax - longMin) * Mathf.Deg2Rad;

        //Store all of the points of the sphere(verts)
        Vector3[] points = new Vector3[rows * columns + columns];

        for (int row = 0; row <= rows; row++)
        {
            float xRatio = row * inverseRows;
            float xRadians = xRatio * latRange + (latMin * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(xRadians);
            float z = radius * Mathf.Cos(xRadians);


            for (int col = 0; col <= columns; col++)
            {
                float yRatio = col * inverseColumns;
                float yRadians = yRatio * longRange + (longMin * Mathf.Deg2Rad);

                Vector3 point = new Vector3(-z * Mathf.Sin(yRadians), y, -z * Mathf.Cos(yRadians));
                Vector3 normal = point * inverseRadius;

                int index = row * columns + (col % columns);
                points[index] = point;
            }
        }

        for (int face = 0; face < (rows * columns); face++)
        {
            int nextFace = face + 1;

            if (nextFace % columns == 0)
            {
                nextFace -= columns;
            }

            Vector3 v0 = position + points[face];
            Vector3 v1 = position + points[face + columns];

            Vector3 v0Next = position + points[nextFace];
            Vector3 v1Next = position + points[nextFace + columns];

            AddLine(v0, v1, color);
            AddLine(v1Next, v1, color);

            if (face % columns == 0 && longRange < (Mathf.PI * 2))
            {
                continue;
            }

            if (isFilled)
            {
                AddTri(v1Next, v0, v0Next, color);
                AddTri(v1Next, v1, v0, color);
            }
        }

    }

    public static void AddCylinder(Vector3 position, float radius, float halfLength, int segments, Color color, bool isFilled = true)
    {
        float segmentSize = (2 * Mathf.PI) / segments;

        for (int i = 0; i < segments; i++)
        {
            float sinRadius = Mathf.Sin(i * segmentSize) * radius;
            float cosRadius = Mathf.Cos(i * segmentSize) * radius;

            float sinRadiusNext = Mathf.Sin((i + 1) * segmentSize) * radius;
            float cosRadiusNext = Mathf.Cos((i + 1) * segmentSize) * radius;

            Vector3 v0Top = position + new Vector3(0, halfLength, 0);
            Vector3 v1Top = position + new Vector3(sinRadius, halfLength, cosRadius);
            Vector3 v2Top = position + new Vector3(sinRadiusNext, halfLength, cosRadiusNext);

            Vector3 v0Bottom = position + new Vector3(0, -halfLength, 0);
            Vector3 v1Bottom = position + new Vector3(sinRadius, -halfLength, cosRadius);
            Vector3 v2Bottom = position + new Vector3(sinRadiusNext, -halfLength, cosRadiusNext);

            AddLine(v1Top, v2Top, color);
            AddLine(v2Top, v2Bottom, color);
            AddLine(v1Bottom, v2Bottom, color);

            if (isFilled)
            {
                //fill the top
                AddTri(v0Top, v1Top, v2Top, color);

                //fill in the sides
                AddTri(v2Top, v1Top, v1Bottom, color);
                AddTri(v1Bottom, v2Bottom, v2Top, color);

                //fill the bottom
                AddTri(v2Bottom, v1Bottom, v0Bottom, color);

            }

        }

    }

    public static void AddRing(Vector3 position, float innerRadius, float outerRadius, int segments, Color color, bool isFilled = true)
    {
        float segmentSize = (Mathf.PI * 2) / segments;

        for (int i = 0; i < segments; i++)
        {
            Vector3 v0Outer = position + new Vector3(Mathf.Sin(i * segmentSize) * outerRadius, 0, Mathf.Cos(i * segmentSize) * outerRadius);
            Vector3 v1Outer = position + new Vector3(Mathf.Sin((i + 1) * segmentSize) * outerRadius, 0, Mathf.Cos((i + 1) * segmentSize) * outerRadius);
            Vector3 v0Inner = position + new Vector3(Mathf.Sin(i * segmentSize) * innerRadius, 0, Mathf.Cos(i * segmentSize) * innerRadius);
            Vector3 v1Inner = position + new Vector3(Mathf.Sin((i + 1) * segmentSize) * innerRadius, 0, Mathf.Cos((i + 1) * segmentSize) * innerRadius);

            if (isFilled)
            {
                AddTri(v1Outer, v0Outer, v0Inner, color);
                AddTri(v0Inner, v1Inner, v1Outer, color);

                AddTri(v0Inner, v0Outer, v1Outer, color);
                AddTri(v1Outer, v1Inner, v0Inner, color);

            }

            AddLine(v0Inner, v1Inner, color);
            AddLine(v0Outer, v1Outer, color);
        }
    }

    public static void AddDisc(Vector3 position, float radius, int segments, Color color, bool isFilled = true)
    {
        float segmentSize = (Mathf.PI * 2) / segments;

        for (int i = 0; i < segments; i++)
        {
            Vector3 v0 = position + new Vector3(Mathf.Sin(i * segmentSize) * radius, 0, Mathf.Cos(i * segmentSize) * radius);
            Vector3 v1 = position + new Vector3(Mathf.Sin((i + 1) * segmentSize) * radius, 0, Mathf.Cos((i + 1) * segmentSize) * radius);

            if (isFilled)
            {
                AddTri(position, v0, v1, color);
                AddTri(v1, v0, position, color);

            }

            AddLine(v0, v1, color);
        }
    }

    public static void AddArc(Vector3 position, float radius, float rotation, float halfAngle, int segments, Color color, bool isFilled = true)
    {
        float segmentSize = (halfAngle * 2) / segments;

        for (int i = 0; i < segments; i++)
        {
            Vector3 v0 = position + new Vector3(Mathf.Sin(i * segmentSize - halfAngle + rotation) * radius, 0, Mathf.Cos(i * segmentSize - halfAngle + rotation) * radius);
            Vector3 v1 = position + new Vector3(Mathf.Sin((i + 1) * segmentSize - halfAngle + rotation) * radius, 0, Mathf.Cos((i + 1) * segmentSize - halfAngle + rotation) * radius);
            if (isFilled)
            {
                AddTri(position, v0, v1, color);
                AddTri(v1, v0, position, color);

            }

            AddLine(v0, v1, color);
        }
    } 

    void OnDrawGizmos()
    {
        if (instance != null)
        {
            DrawGL();
            ClearGL();
        }
    }
    void DrawGL()
    {
        //Push the matrix
        GL.PushMatrix();

        //Set thematerial Pass to 0
        instance.material.SetPass(0);

        // Begin drawing lines
        GL.Begin(GL.LINES);

        Line[] lines = instance.lines;
        for (int i = 0; i < instance.lineIndex; i++)
        {
            Line line = lines[i];
            DrawVertex(line.v0);
            DrawVertex(line.v1);
        }

        // End drawing lines
        GL.End();

        //Begin drawing tris
        GL.Begin(GL.TRIANGLES);

        Tri[] tris = instance.tris;
        for (int i = 0; i < instance.triIndex; i++)
        {
            Tri tri = tris[i];
            DrawVertex(tri.v0);
            DrawVertex(tri.v1);
            DrawVertex(tri.v2);
        }
        //End drawing tris
        GL.End();

        //Pop the matrix
        GL.PopMatrix();


    }
    void ClearGL()
    {
        instance.lineIndex = 0;
        instance.triIndex = 0;

    }
    void DrawVertex(Vertex v)
    {
        GL.Color(v.colour);
        GL.Vertex(v.position);
    }
}
