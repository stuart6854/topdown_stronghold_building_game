using System.Collections;
using System.Collections.Generic;

public class Inventory {

    private List<LooseItem> _Inventory;
    private int Capacity; //-1 signifies infinite size

    public Inventory(int maxStacks = -1) {
        this.Capacity = maxStacks;

        if(this.Capacity > -1) this._Inventory = new List<LooseItem>(this.Capacity);
        else this._Inventory = new List<LooseItem>();
    }

    public LooseItem Add(LooseItem stack) {
        if(stack != null && stack.GetStackSize() > 0)
            stack = TryAddToExistingStack(stack);

        if(stack != null && stack.GetStackSize() > 0)
            stack = TryAddNewStack(stack);

        //If their is still some items in passed stack then return them,
        //else run null to signify the we managed to insert the whole stack into our inventory
        return stack;
    }

    public LooseItem Remove(string type, int amnt) {
        LooseItem stack = new LooseItem(type, 0);

        List<LooseItem> RemoveList = new List<LooseItem>(); //Empty stacks to be removed

        LooseItem[] existingStacks = GetExistingStacks(type);
        if(existingStacks.Length == 0)
            return null;//Their are no stacks to take from

        foreach(LooseItem existingStack in existingStacks) {
            if(existingStack.GetStackSize() <= 0) {
                //Cannot take from this empty stack, so mark it to be removed from inventory
                // and continue to next stack
                RemoveList.Add(existingStack);
                continue;
            }

            if(existingStack.GetStackSize() >= amnt) {
                //This stack has enough for the whole remaining amnt
                stack.AddToStack(amnt);
                existingStack.RemoveFromStack(amnt);
                amnt = 0;
            } else {
                //There is not enough in extistingStack to fill whole request,
                //so take what we can
                stack.AddToStack(existingStack.GetStackSize());
                amnt -= existingStack.GetStackSize();
                existingStack.SetStackSize(0); //We took the whole stack. Nothing left
            }

            if(existingStack.GetStackSize() <= 0)
                RemoveList.Add(existingStack); //We have emptied this stack so mark it for removal from inventory

            if(amnt == 0)
                break; //We have gotten the requested amnnt. We are done
        }

        foreach(LooseItem removeStack in RemoveList) {
            _Inventory.Remove(removeStack); //Removes the empty stacks from inventory
        }

        if(stack.GetStackSize() <= 0)
            return null;//There wasnt any of objects of that type in the inventory

        return stack;
    }

    public bool Contains(string type, int amnt = 1) {
        foreach(LooseItem stack in _Inventory) {
//            if(stack.GetObjectType() == type && stack.GetStackSize() >= amnt)
//                return true;
        }

        return false;
    }

    private LooseItem TryAddToExistingStack(LooseItem stack) {
//        LooseItem[] existingStacks = GetExistingStacks(stack.GetObjectType());
//        if(existingStacks.Length == 0)
//            return stack;//Their is no existing stack, so return the stack as given

//        foreach(LooseItem existingStack in existingStacks) {
//            if(existingStack.GetStackSize() == existingStack.GetMaxStackSize())
//                continue; //There is no more space to add any of the stack at all, lets try the next stack(if one)
//
//            int spaceLeft = existingStack.GetMaxStackSize() - existingStack.GetStackSize(); //Get the existing stacks available space
//            if(spaceLeft >= stack.GetStackSize()) {
//                //There is enough room to add the whole stack
//                existingStack.AddToStack(stack.GetStackSize());
//                return null; //We managed to add the whole/remaining stack to the existing stack, so the passed stack no longer exists
//            }
//
//            //There is not enough room for the whole stack,
//            //so add what we can
//            existingStack.AddToStack(spaceLeft);
//            stack.RemoveFromStack(spaceLeft);
//
//            if(stack.GetStackSize() <= 0)
//                return null; //The stack has been completly added to other stacks
//        }

        return stack;
    }

    private LooseItem TryAddNewStack(LooseItem stack) {
        //At this point either there isnt any existing stack of this type or
        //we managed to add to a existing stack but couldnt add the whole stack
        if(_Inventory.Count >= Capacity && Capacity > -1)//Check if the inventory is full
            return stack; //The inventory is full

        _Inventory.Add(stack);
        return null; //We managed to add the whole stack to this inventory, so the passed stack no longer exists
    }

    private LooseItem[] GetExistingStacks(string type) {
        List<LooseItem> existing = new List<LooseItem>();

        foreach(LooseItem stack in _Inventory) {
//            if(stack.GetObjectType() != type)
//                continue;

            existing.Add(stack);
        }

        return existing.ToArray();
    }

}
