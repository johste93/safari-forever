using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class RailNode : MonoBehaviour
{
        public NodeType type;
        public Transform child;
        public SpriteRenderer nodeSurface;
        public SpriteRenderer[] nodeRenderers;
        public new CircleCollider2D collider;
        public LayerMask whatIsRailNode;
        public GameObject linkPrefab;

        [NonSerialized] public LevelEntity levelEntity;

        protected List<Tween> tweens = new List<Tween>();
        [HideInInspector] public List<RailLink> links = new List<RailLink>();

        protected int assignedFingerIndex = -1;
        protected Collider2D[] overlappingNodes = new Collider2D[9];


        public virtual void Initalize(LevelEntity parentEntity)
        {
            levelEntity = parentEntity;
            transform.position = levelEntity.gizmo.transform.position;

            child.gameObject.SetActive(false);
            collider.enabled = false;
        }

        public void SetVisible(bool visible)
        {
            collider.enabled = visible;

            child.DOComplete();
            child.localScale = Vector3.one * (visible ? 0f : 0.6f);
            foreach(SpriteRenderer sR in nodeRenderers)
                sR.color = sR.color.SetAlpha(visible ? 1f : 0.6f);

            if(visible)
            {
                
                //child.localScale = Vector3.zero;
                tweens.Add(
                    child.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack)
                );
            }
        }

        public void AssignFinger(int fingerIndex)
        {
            assignedFingerIndex = fingerIndex;
        }

        protected virtual void On_TouchStart(TouchInfo touch)
        {
            if (touch.pickedUIElement != null)
                return;

            if(!touch.GetAllPickedColliders(Camera.main).Contains(collider))
                return;

            Vector2 worldPos = touch.GetTouchToWorldPoint(10, Camera.main);

            if (FindClosestNode(worldPos, true) != this)
			    return;

            AssignFinger(touch.fingerIndex);
        }

        protected virtual void On_TouchMaintained(TouchInfo touch)
        {
            if (assignedFingerIndex != touch.fingerIndex)
                return;
        }

        protected virtual void On_TouchEnd(TouchInfo touch)
        {
            if (assignedFingerIndex != touch.fingerIndex)
                return;

            if(touch.duration < Globals.gameConstants.standardTapDuration)
            {
                On_Tap(touch);
            }

            assignedFingerIndex = -1;
        }

        protected virtual void On_Tap(TouchInfo touch)
        {
        }

        protected void KillAllTweens()
        {
            foreach (Tween t in tweens)
            {
                t.Kill();
            }
            tweens = new List<Tween>();
        }

        private void RevealNodes(NodeType type, LevelEntity sourceEntity)
        {
            Debug.Log(levelEntity);
            bool hasTwoNodes = levelEntity.railNodeOutput != null && levelEntity.railNodeInput != null;
            

            child.gameObject.SetActive(!hasTwoNodes || (hasTwoNodes && type == this.type));
            SetVisible(this.type == type && sourceEntity != levelEntity);
        }

        private void HideNodes()
        {
            SetVisible(false);
            child.gameObject.SetActive(false);
        }

        protected RailNode FindClosestNode(Vector2 position, bool allowSelf)
        {
            overlappingNodes = new Collider2D[9];
            if (Physics2D.OverlapPointNonAlloc(position, overlappingNodes, whatIsRailNode) > 0)
            {
                Collider2D closestNode = null;
                float closestNodeDistance = float.MaxValue;
                foreach (Collider2D nodeCollider in overlappingNodes)
                {
                    if(nodeCollider == null || (!allowSelf && nodeCollider == collider))
                        continue;

                    if( levelEntity.railNodeInput != null )
                        if( nodeCollider == levelEntity.railNodeInput.collider )
                            continue;

                    float distance = Vector2.Distance(position, nodeCollider.transform.position);
                    if (distance < closestNodeDistance)
                    {
                        closestNode = nodeCollider;
                        closestNodeDistance = distance;
                    }
                }
                if(closestNode == null)
                    return null;
                    
                return closestNode.GetComponentInParent<RailNode>();
            }
            return null;
        }

        public List<string> GetConnectedEntities()
        {
            List<string> result = new List<string>();
            foreach(RailLink link in links)
                result.Add(link.OtherNode(this).levelEntity.uniqueId);

            return result;
        }

        public bool AlreadyLinked(RailNode other)
        {
            foreach(RailLink link in links)
            {
                if(link.OtherNode(this) == other)
                    return true;
            }

            return false;
        }

        public void AddLink(RailLink newLink)
        {
            links.Add(newLink);
        }

        public void RemoveLink(RailLink removedLink)
        {
            links.Remove(removedLink);
        }

        protected virtual void OnEnable()
        {
            RailNodeManager.RevealNodes += RevealNodes;
            RailNodeManager.HideNodes += HideNodes;

            TouchInput.On_TouchStart += On_TouchStart;
            TouchInput.On_TouchMaintained += On_TouchMaintained;
            TouchInput.On_TouchEnd += On_TouchEnd;
        }

        protected virtual void Unsubscribe()
        {
            RailNodeManager.RevealNodes -= RevealNodes;
            RailNodeManager.HideNodes -= HideNodes;

            TouchInput.On_TouchStart -= On_TouchStart;
            TouchInput.On_TouchMaintained -= On_TouchMaintained;
            TouchInput.On_TouchEnd -= On_TouchEnd;
        }

        protected virtual void OnDisable()
        {
            Unsubscribe();
            KillAllTweens();
        }

        protected virtual void OnDestroy()
        {
            Unsubscribe();
            KillAllTweens();
        }
}

