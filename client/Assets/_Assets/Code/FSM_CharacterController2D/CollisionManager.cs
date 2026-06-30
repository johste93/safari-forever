using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM_CharacterController2D
{
    public class CollisionManager
    {   
        private FSM_CharacterController characterController;

        private Dictionary<IInteractable, CollisionEntry> collisionsPreviousFrame = new Dictionary<IInteractable, CollisionEntry>();
        private Dictionary<IInteractable, CollisionEntry> collisionsThisFrame = new Dictionary<IInteractable, CollisionEntry>();

        public void SetReferenceToCharacter(FSM_CharacterController characterController)
		{
			this.characterController = characterController;
		}

        public void Prepare()
        {
            collisionsPreviousFrame = collisionsThisFrame;
            collisionsThisFrame = new Dictionary<IInteractable, CollisionEntry>();
        }

        public void RegisterCollision(IInteractable obj, Direction4 direction)
        {
            if(!collisionsThisFrame.ContainsKey(obj))
                collisionsThisFrame.Add(obj, new CollisionEntry(obj, characterController, direction));
        }

        public void UpdateCollidables()
        {
            List<CollisionEntry> maintainedCollisions = collisionsThisFrame.Where(x => collisionsPreviousFrame.ContainsKey(x.Key)).Select(x => x.Value).ToList();
            List<CollisionEntry> newCollisions =        collisionsThisFrame.Where(x => !maintainedCollisions.Contains(x.Value)).Select(x => x.Value).ToList();
            List<CollisionEntry> lostCollisions =       collisionsPreviousFrame.Where(x => !newCollisions.Contains(x.Value)).Select(x => x.Value).ToList();

            foreach(CollisionEntry collisionEntry in newCollisions)
                collisionEntry.obj.NewCollision(collisionEntry);

            foreach(CollisionEntry collisionEntry in maintainedCollisions)
                collisionEntry.obj.MaintainedCollision(collisionEntry);

            foreach(CollisionEntry collisionEntry in lostCollisions)
                collisionEntry.obj.LostCollision(collisionEntry);

            collisionsPreviousFrame = collisionsThisFrame;
        }
    }
}
