using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.LogicSystem.v2
{
    public class Link : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public LineRenderer arrowRenderer;
        public EdgeCollider2D edgeCollider;
        
        [HideInInspector] public bool isSelected;
        [HideInInspector] public int lastInterception = -1;
        [HideInInspector] public int lastHeld = -1;
        [HideInInspector] public Node heldNode;
        
        public OutputNode nodeA;
        public InputNode nodeB;

        private Color linkColor = Color.white;
        private Color selectedColor = new Color(1,0.6f,0,1);

        private Vector3[] localArrowOffset = {
            new Vector3(-0.25f, -0.125f, 0f),
            new Vector3(0f, 0.125f, 0f),
            new Vector3(0.25f, -0.125f, 0f)
        };

        public void EmitPower(bool emit)
        {
            nodeB.Power(emit);
        }

        public void Initalize(OutputNode sourceNode)
        {
            this.nodeA = sourceNode;
            UpdateLineRenderer(nodeA.transform.position, nodeA.transform.position);
            SetVisible(LogicCanvas.LogicVisible());
            
            nodeA.levelEntity.On_DestroySelf += On_DestroySelf;
        }

        public void UpdatePosition(Vector2 position)
        {
            UpdateLineRenderer(nodeA.transform.position, position);
        }

        public void Attach(InputNode targetNode)
        {
            if(nodeA.AlreadyLinked(targetNode))
            {
                Detach();
                return;
            }

            this.nodeB = targetNode;
            UpdateLineRenderer(nodeA.transform.position, nodeB.transform.position);
            UpdateCollider();

            nodeA.AddLink(this);
            nodeB.AddLink(this);

            linkColor = Color.white.SetAlpha(0.70f);
            lineRenderer.sortingOrder = 0;

            nodeB.levelEntity.On_DestroySelf += On_DestroySelf;
        }

        private void UpdateLineRenderer(Vector2 start, Vector2 target)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, target);

            UpdateArrowPosition(start, target);
        }

        private void UpdateArrowPosition(Vector2 start, Vector2 target)
        {
            Vector3 dir = (target - start).normalized;
            Vector3 centerPos = Vector3.Lerp(start, target, 0.5f) + (dir * 0.25f);

            float ang = Vector2.Angle(Vector2.up, dir);
            Vector3 cross = Vector3.Cross(Vector2.up, dir);
            
            if (cross.z < 0)
                ang = 360 - ang;

            for(int i = 0; i < 3; i++)
                arrowRenderer.SetPosition(i, centerPos + (Quaternion.Euler(0, 0, ang) * localArrowOffset[i]));
        }

        private void UpdateCollider()
        {
            Vector2[] colliderpoints = new Vector2[2];
            colliderpoints[0] = transform.InverseTransformPoint( lineRenderer.GetPosition(0) );
            colliderpoints[1] = transform.InverseTransformPoint( lineRenderer.GetPosition(1) );
            edgeCollider.points = colliderpoints;
        }

        public void Detach()
        {
            if(nodeA != null)
            {
                nodeA.RemoveLink(this);
                nodeA.levelEntity.On_DestroySelf -= On_DestroySelf;
            }

            if(nodeB != null)
            {
                nodeB.RemoveLink(this);
                nodeB.levelEntity.On_DestroySelf -= On_DestroySelf;
            }

            Destroy(gameObject);
        }

        public Node OtherNode(Node sourceNode)
        {
            if(sourceNode == nodeA)
                return nodeB;
            
            return nodeA;
        }

        public void SetVisible(bool visible)
        {
            lineRenderer.enabled = visible;
            arrowRenderer.enabled = visible;
            edgeCollider.enabled = visible;
        }

        public void SetColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            arrowRenderer.startColor = color;
            arrowRenderer.endColor = color;
        }

        private void LateUpdate()
        {
            if(nodeB == null)
                return;

            if(isSelected)
            {
                SetColor(selectedColor);

                if(TouchInput.GetTouchCount() > 0)
                    isSelected = false;

                return;
            }

            if(lastInterception == Time.frameCount || lastHeld == Time.frameCount)
                SetColor(lastInterception == Time.frameCount ? Color.red : heldNode.nodeSurface.color);
            else
                SetColor(linkColor);
        }

        private void On_EntityMoved(LevelEntity levelEntity)
        {
            if(levelEntity == nodeA.levelEntity)
            {
                UpdateLineRenderer(nodeA.transform.position, lineRenderer.GetPosition(1));
                UpdateCollider();
            }

            if(levelEntity == nodeB.levelEntity)
            {
                UpdateLineRenderer(lineRenderer.GetPosition(0), nodeB.transform.position);
                UpdateCollider();
            }
        }

        private void UpdateLinkPositions()
        {

        }

        private void RevealNodes(NodeType type, LevelEntity sourceNode)
        {
            SetVisible(true);
        }

        private void HideNodes()
        {
            SetVisible(false);
        }

        private void On_DestroySelf()
        {
            Detach();
        }

        private void On_LogicCanvasUpdate(bool visible)
        {
            if(nodeA == null || nodeB == null)
                return;

            UpdateLineRenderer(nodeA.transform.position, nodeB.transform.position);
            UpdateCollider();
        }

        protected void OnEnable()
        {
            NodeManager.RevealNodes += RevealNodes;
            NodeManager.HideNodes += HideNodes;

            LevelEntity.On_EntityMoved += On_EntityMoved;
            LevelEntity.On_EntityStoppedChangingSize += On_EntityMoved;
            LogicCanvas.On_LogicCanvasUpdate += On_LogicCanvasUpdate;
        }

        protected void Unsubscribe()
        {
            NodeManager.RevealNodes -= RevealNodes;
            NodeManager.HideNodes -= HideNodes;

            LevelEntity.On_EntityMoved -= On_EntityMoved;
            LevelEntity.On_EntityStoppedChangingSize -= On_EntityMoved;
            LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;
        }

        protected void OnDisable()
        {
            Unsubscribe();
        }

        protected void OnDestroy()
        {
            Unsubscribe();
        }
    }
}