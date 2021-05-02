using System;
using System.Collections.Generic;
using System.Linq;

namespace DijkstraAlgorithm
{	
	public class Edge
	{
		public readonly Node From;
		public readonly Node To;
		public Edge(Node first, Node second)
		{
			this.From = first;
			this.To = second;
		}
		public bool IsIncident(Node node)
		{
			return From == node || To == node;
		}
		public Node OtherNode(Node node)
		{
			if (!IsIncident(node)) throw new ArgumentException();
			if (From == node) return To;
			return From;
		}
	}

	public class Node
	{
		readonly List<Edge> edges = new List<Edge>();
		public readonly int NodeNumber;

		public Node(int number)
		{
			NodeNumber = number;
		}

		public IEnumerable<Node> IncidentNodes
		{
			get
			{
				return edges.Select(z => z.OtherNode(this));
			}
		}
		public IEnumerable<Edge> IncidentEdges
		{
			get
			{
				foreach (var e in edges) yield return e;
			}
		}
		public static Edge Connect(Node node1, Node node2, Graph graph)
		{
			if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
			var edge = new Edge(node1, node2);
			node1.edges.Add(edge);
			node2.edges.Add(edge);
			return edge;
		}
		public static void Disconnect(Edge edge)
		{
			edge.From.edges.Remove(edge);
			edge.To.edges.Remove(edge);
		}
	}

	public class Graph
	{
		private Node[] nodes;

		public Graph(int nodesCount)
		{
			nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
		}

		public int Length { get { return nodes.Length; } }

		public Node this[int index] { get { return nodes[index]; } }

		public IEnumerable<Node> Nodes
		{
			get
			{
				foreach (var node in nodes) yield return node;
			}
		}

		public void Connect(int index1, int index2)
		{
			Node.Connect(nodes[index1], nodes[index2], this);
		}
		
		public Edge GetConnectedEdge(int index1, int index2)
		{
			return Node.Connect(nodes[index1], nodes[index2], this);
		}

		public void Delete(Edge edge)
		{
			Node.Disconnect(edge);
		}

		public IEnumerable<Edge> Edges
		{
			get
			{
				return nodes.SelectMany(z => z.IncidentEdges).Distinct();
			}
		}

		public static Graph MakeGraph(params int[] incidentNodes)
		{
			var graph = new Graph(incidentNodes.Max() + 1);
			for (int i = 0; i < incidentNodes.Length - 1; i += 2)
				graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
			return graph;
		}
	}

	class DijkstraData
	{
		public Node Previous { get; set; }
		public double Price { get; set; }
	}

	public class Program
    {
	    public static List<Node> Dijkstra(Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
	    {
		    var notVisited = graph.Nodes.ToList();
		    var track = new Dictionary<Node, DijkstraData>();
		    track[start] = new DijkstraData { Price = 0, Previous = null };

		    while (true)
		    {
			    Node toOpen = null;
			    var bestPrice = double.PositiveInfinity;
			    foreach (var e in notVisited)
			    {
				    if (track.ContainsKey(e) && track[e].Price < bestPrice)
				    {
					    bestPrice = track[e].Price;
					    toOpen = e;
				    }
			    }

			    if (toOpen == null) return null;
			    if (toOpen == end) break;

			    foreach (var e in toOpen.IncidentEdges.Where(z => z.From == toOpen))
			    {
				    var currentPrice = track[toOpen].Price + weights[e];
				    var nextNode = e.OtherNode(toOpen);
				    if (!track.ContainsKey(nextNode) || track[nextNode].Price > currentPrice)
				    {
					    track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
				    }
			    }

			    notVisited.Remove(toOpen);
		    }

		    var result = new List<Node>();
		    while (end != null)
		    {
			    result.Add(end);
			    end = track[end].Previous;
		    }
		    result.Reverse();
		    return result;
	    }
	    
	    public static void Main()
	    {
		    var graph = new Graph(4);
		    var weights = new Dictionary<Edge, double>();
		    weights[graph.GetConnectedEdge(0, 1)] = 1;
		    weights[graph.GetConnectedEdge(0, 2)] = 2;
		    weights[graph.GetConnectedEdge(0, 3)] = 6;
		    weights[graph.GetConnectedEdge(1, 3)] = 4;
		    weights[graph.GetConnectedEdge(2, 3)] = 2;

		    var path = Dijkstra(graph, weights, graph[0], graph[3]).Select(n => n.NodeNumber);
	    }
    }
}
