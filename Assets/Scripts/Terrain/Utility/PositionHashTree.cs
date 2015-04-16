﻿using UnityEngine;
using System.Collections;

public class PositionHashTree
{

    class HTNode
    {
        //Members
        string hash;
        TerrainBlock block;
        //children of this node
        public HTNode left;
        public HTNode right;
        //parent of this node (needed for removal funcs)
        public HTNode parent;

        //Constructors
        public HTNode(string h, TerrainBlock b)
        {
            hash = h;
            block = b;
        }

        //Functions
        //Destroy a node and update the tree
        public void dispose()
        {
            bool isLeftChild = (parent.left == this);
            if(countChildren() == 0) // this node has no children
            {
                if (isLeftChild)
                    parent.left = null;
                else
                    parent.right = null;
            }
            else if (countChildren() == 1) // this node has one child
            {
                //"this" references are used for the sake of clarity
                if (left != null) //the one child is our left child
                {
                    left.parent = this.parent;
                    if (isLeftChild)
                        parent.left = this.left;
                    else
                        parent.right = this.left;
                }
                else //the one child is our right child
                {
                    right.parent = this.parent;
                    if (isLeftChild)
                        parent.left = this.right;
                    else
                        parent.right = this.right;
                }
            }
            else //this node has two children
            {
                //find minimum of right tree
                HTNode min = right.minimum();
                block = min.block;
                hash = min.hash;
                min.parent.left = null;
            }
        }

        public int countChildren()
        {
            int count = 0;
            if(left != null)
                count++;
            if (right != null)
                count++;
            return count;
        }

        //finds minimum value in the tree
        public HTNode minimum()
        {
            if(left == null)
                return this;
            else
                return left.minimum();
        }

        public TerrainBlock getBlock()
        {
            return block;
        }

        public void setBlock(TerrainBlock b)
        {
            block = b;
        }

        public string getHash()
        {
            return hash;
        }

        public void setHash(string h)
        {
            hash = h;
        }

        //pass root of BST to this function
        public HTNode findBlock(string hash)
        {
            HTNode targetNode = this;
            while (targetNode != null)
            {
                int cmp = string.Compare(targetNode.hash, hash);
                if (cmp < 0) //this node does not contain the desired string; should be to the left if it occurs at all
                    targetNode = targetNode.left;
                else if (cmp > 0) //same deal but to the right
                    targetNode = targetNode.right;
                else
                    return targetNode; //exact match
            }
            return null; //no match
        }

        //pass root of BST by reference to this function
        public bool bstInsert(ref HTNode current)
        {
            if (current == null)
            {
                current = this;
            }
            else if (string.Compare(current.hash, this.hash) < 0)
            { //less than previous node; go left
                if (bstInsert(ref current.left) && (current.left.left == null && current.left.right == null))
                {
                    current.left.parent = current;
                    return true;
                }
            }
            else if (string.Compare(current.hash, this.hash) > 0)
            { //greater than previous node; go right
                if (bstInsert(ref current.right) && (current.right.left == null && current.right.right == null))
                {
                    current.right.parent = current;
                    return true;
                }
            }
            else
            {
                Debug.LogError("Duplicate hash tree node");
                return false; //equal to previous node; no duplicates
            }
            return true; //inserted successfully
        }
    }; //class HTNode

    HTNode root;

    public PositionHashTree()
    {
        root = new HTNode("0", null);
    }

    public PositionHashTree(TerrainBlock mergeableBlock)
    {
        root = null;
        addBlock(mergeableBlock);
    }

    private static string hashPosition(TerrainBlock mergeableBlock)
    {
        Vector3 vec;
        vec.x = mergeableBlock.transform.position.x;
        vec.y = mergeableBlock.transform.position.y;
        vec.z = mergeableBlock.transform.position.z;

        return hashPosition(vec);
    }

    private static string hashPosition(Vector3 position)
    {
        if (position == null)
            return null;
        string hash = "";

        hash += Mathf.RoundToInt(position.x).ToString();
        hash += Mathf.RoundToInt(position.y).ToString();
        hash += Mathf.RoundToInt(position.z).ToString();

        return hash;
    }

    public bool addBlock(TerrainBlock mergeableBlock)
    {
        string hash = hashPosition(mergeableBlock);
        HTNode newNode = new HTNode(hash, mergeableBlock);
        if (!newNode.bstInsert(ref root))
        {
            Debug.LogError("PositionHashTree::addBlock() - failed to insert new node into tree");
            return false;
        }
        return true;
    }

    public TerrainBlock findBlock(Vector3 pos)
    {
        string hash = hashPosition(pos);
        HTNode node = root.findBlock(hash);
        if (node == null)
            return null;
        else
            return node.getBlock();
    }

    public bool removeBlock(Vector3 pos)
    {
        HTNode node = root.findBlock(hashPosition(pos));
        if (node == null)
        {
            Debug.LogError("PositionHashTree::removeBlock(Vector3) - Could not find block to be removed");
            return false;
        }
        if (node == root) //root must be handled differently
        {
            if (node.countChildren() == 0)
                root = null;
            else if (node.countChildren() == 1)
            {
                if (root.left != null)
                    root = root.left;
                else
                    root = root.right;
            }
            else
            {
                HTNode target = root.right.minimum();
                target.parent.left = null;
                root.setBlock(target.getBlock());
                root.setHash(target.getHash());
            }
            return true;
        }
        node.dispose();
        return true;
    }
}
