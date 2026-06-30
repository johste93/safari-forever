using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace SF.LogicSystem.v2
{
    public class OutputNode : Node
    {
        private Link unattachedLink;
        private Node lastTargetNode;

        private bool dragStarted;
        private float heldDuration = 0;
        private float detachDuration = 1.5f;


        protected override void On_TouchStart(TouchInfo touch)
        {
            if (touch.pickedUIElement != null)
                return;

            //if (touch.GetFirstPickedGameObject(Camera.main) != collider.gameObject)
			//    return;

            base.On_TouchStart(touch);
        }

        public virtual void EmitPower(bool emit)
        {
            foreach(Link link in links)
            {
                link.EmitPower(emit);
            }
        }

        protected void OnDragStart(TouchInfo touch)
        {
            if(dragStarted)
                return;

            dragStarted = true;
            unattachedLink = SpawnLink();
            NodeManager.RevealNodes?.Invoke(NodeType.Input, levelEntity);

            Audio.Play(SFX.instance.level.logicSwitch.create, Channel.Game);
        }

        protected virtual void OnHeld(TouchInfo touch)
        {
            heldDuration += Time.deltaTime;
        
            if(heldDuration > 0.3f)
            {
                float t = Mathf.Clamp01((heldDuration-0.3f) / detachDuration);
                child.localScale = Vector3.one * Mathf.Lerp(1f, 2f, t);
                nodeSurface.color = Color.Lerp(Color.white, Color.red, t);

                if(levelEntity.GetAttachedLinks().Count > 0)
                    foreach(Link link in new List<Link>(levelEntity.GetAttachedLinks()))
                    {
                        link.heldNode = this;
                        link.lastHeld = Time.frameCount;
                    }
            }

            if(heldDuration > detachDuration)
            {
                if(levelEntity.GetAttachedLinks().Count > 0)
                    foreach(Link link in new List<Link>(levelEntity.GetAttachedLinks()))
                        link.Detach();

                Audio.Play(SFX.instance.level.logicSwitch.detach, Channel.Game);
                On_TouchEnd(touch);
            }
        }

        protected override void On_TouchMaintained(TouchInfo touch)
        {
            if (assignedFingerIndex != touch.fingerIndex)
                return;

            base.On_TouchMaintained(touch);
            
            if(Vector2.Distance(touch.viewportStartPosition, touch.viewportPosition) > 0.015f)
                OnDragStart(touch);

            if (unattachedLink == null)
            {
                OnHeld(touch);
                return;
            }

            Vector2 worldPos = touch.GetTouchToWorldPoint(10, Camera.main);

            unattachedLink.UpdatePosition(worldPos);

            InputNode targetNode = (InputNode) FindClosestNode(worldPos, false);
            
            if(targetNode != lastTargetNode)
            {
                if(lastTargetNode != null)
                    lastTargetNode.child.localScale = Vector3.one;

                lastTargetNode = targetNode;
            }
            
            if(targetNode != null)
                targetNode.child.localScale = Vector3.MoveTowards(targetNode.child.localScale, Vector3.one * 1.5f, Time.deltaTime * 10f);
        }

        protected override void On_TouchEnd(TouchInfo touch)
        {
            if (assignedFingerIndex != touch.fingerIndex)
                return;

            base.On_TouchEnd(touch);

            Reset();

            if (unattachedLink == null)
                return;

            Vector2 worldPos = touch.GetTouchToWorldPoint(10, Camera.main);
            InputNode targetNode = (InputNode) FindClosestNode(worldPos, false);

            if(targetNode == null)
            {
                unattachedLink.Detach();
            }
            else
            {
                Audio.Play(SFX.instance.level.logicSwitch.attach, Channel.Game);
                unattachedLink.Attach(targetNode);
            }

            unattachedLink = null;

            if(lastTargetNode != null)
            {
                lastTargetNode.child.localScale = Vector3.one;
                lastTargetNode = null;
            }

            NodeManager.RevealNodes?.Invoke(NodeType.Output, null);
        }

        protected override void On_Tap(TouchInfo info)
        {
            if(levelEntity.GetAttachedLinks().Count > 0)
                foreach(Link link in new List<Link>(levelEntity.GetAttachedLinks()))
                    link.isSelected = true;

            child.DOComplete();
            child.DOPunchScale(Vector3.one * 0.3f, 0.3f, 1);
            Audio.Play(SFX.instance.level.logicSwitch.detach, Channel.Game);
        }

        private void Reset()
        {
            dragStarted = false;
            heldDuration = 0f;
            child.localScale = Vector3.one;
            nodeSurface.color = Color.white;
        }

        protected Link SpawnLink()
        {
            GameObject lastSpawn = Instantiate(linkPrefab);
            Link link = lastSpawn.GetComponent<Link>();
            link.Initalize(this);
            return link;
        }

        public virtual void Deserialize()
        {
            List<LevelEntity> levelEntites = LevelBuilder.instance.GetLevelEntities();
            foreach(string uniqueId in levelEntity.serializableData.connectedEntities)
            {
                LevelEntity other = levelEntites.FirstOrDefault(x => x.uniqueId == uniqueId);
                Link link = SpawnLink();

                link.Attach(other.inputNode);
                link.SetColor(Color.white);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var links = levelEntity.GetAttachedLinks();
            for(int i = 0; i < links.Count; i++)
            {
                if(links[i] != null)
                    links[i].Detach();
            }
        }
    }
}