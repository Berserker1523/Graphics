using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental;
using UnityEditor.Experimental.Graph;
using Object = UnityEngine.Object;

namespace UnityEditor.Experimental
{
    internal class VFXEdDataSource : ScriptableObject, ICanvasDataSource
    {
        private List<CanvasElement> m_Elements = new List<CanvasElement>();

        public void OnEnable()
        {
            VFXEditor.BlockLibrary.Load(); // Force a reload
        }

        public void AddNode(VFXEdNode n)
        {
            m_Elements.Add(n);
        }

        public void UndoSnapshot(string Message)
        {
            // TODO : Make RecordObject work (not working, no errors, have to investigate)
            Undo.RecordObject(this, Message);
        }

        public CanvasElement[] FetchElements()
        {
            return m_Elements.ToArray();
        }

        public void DeleteElement(CanvasElement e)
        {
            Canvas2D canvas = e.ParentCanvas();
            m_Elements.Remove(e);
            canvas.ReloadData();
            canvas.Repaint();

        }

        public void Connect(VFXEdDataAnchor a, VFXEdDataAnchor b)
        {
			m_Elements.Add(new Edge<VFXEdDataAnchor>(this, a, b));
        }

        public bool ConnectFlow(VFXEdFlowAnchor a, VFXEdFlowAnchor b)
        {
			VFXContextModel model0 = a.FindParent<VFXEdNode>().Model;
			VFXContextModel model1 = b.FindParent<VFXEdNode>().Model;

			if (a.GetDirection() == Direction.Input)
			{
				VFXContextModel tmp = model0;
				model0 = model1;
				model1 = tmp; 
			}

			if (!VFXSystemModel.ConnectContext(model0, model1))
				return false;

			var edgesToErase = new List<FlowEdge<VFXEdFlowAnchor>>();
			foreach (CanvasElement element in  m_Elements)
			{
				FlowEdge<VFXEdFlowAnchor> edge = element as FlowEdge<VFXEdFlowAnchor>;
				if (edge != null && (edge.Left == a || edge.Right == a || edge.Left == b || edge.Right == b))
					edgesToErase.Add(edge);
			}

			foreach (var edge in edgesToErase)
				m_Elements.Remove(edge);

            m_Elements.Add(new FlowEdge<VFXEdFlowAnchor>(this, a, b));
			return true;
        }

        public void AddEmptyNode(object o)
        {
            VFXEdSpawnData data = o as VFXEdSpawnData;
            VFXEdNode node = new VFXEdNode(data.mousePosition,data.context, this);
            AddNode(node);
            data.targetCanvas.ReloadData();

        }

        public void AddSpecificNode(object o)
        {
            VFXEdSpawnData data = o as VFXEdSpawnData;
            VFXEdNode node = new VFXEdNode(data.mousePosition, data.context, this);
            node.AddNodeBlock(VFXEditor.BlockLibrary.GetBlock(data.libraryName));
            AddNode(node);
            data.targetCanvas.ReloadData();
        }
    }
}

