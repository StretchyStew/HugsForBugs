﻿// Skeleton implementation by: Joe Zachary, Daniel Kopta, Travis Martin for CS 3500
// Last updated: August 2023 (small tweak to API)

using System;

namespace SpreadsheetUtilities;

/// <summary>
/// (s1,t1) is an ordered pair of strings
/// t1 depends on s1; s1 must be evaluated before t1
/// 
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        (The set of things that depend on s)    
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///        (The set of things that s depends on) 
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}
/// </summary>
public class DependencyGraph
{

    private Dictionary<string, HashSet<string>> dependent_graph, dependee_graph;

    private int dependencyGraphSize { get; set; }

    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        dependent_graph = new Dictionary<string, HashSet<string>>();
        dependee_graph = new Dictionary<string, HashSet<string>>();
        int dependencyGraphSize;
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// This is an example of a property.
    /// </summary>
    public int NumDependencies
    {
        get { return dependencyGraphSize; }
    }


    /// <summary>
    /// Returns the size of dependees(s),
    /// that is, the number of things that s depends on.
    /// </summary>
    public int NumDependees(string s)
    {
        int numOfDependees = 0;
        foreach (string key in dependee_graph.Keys)
        {
            if (dependee_graph.ContainsKey(s))
            {
                numOfDependees++;
            }
        }
        return numOfDependees;
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        if (dependent_graph.ContainsKey(s))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        if (dependee_graph.ContainsKey(s))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        if (dependent_graph.ContainsKey(s))
        {
            return new HashSet<string>(dependent_graph[s]);
        }
        return new HashSet<string>();
    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        if (dependee_graph.ContainsKey(s))
        {
            return new HashSet<string>(dependee_graph[s]);
        }
        return new HashSet<string>();
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///   s is dependee
    ///
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>
    public void AddDependency(string s, string t)
    {
        //if 's' is already in dependents, then it will just add it
        if (dependent_graph.ContainsKey(s))
        {
            dependee_graph[s].Add(t);
        }
        //if 's' ins't in dependents, then this will create a new HashSet for the dependee and assign it to t.
        else
        {
            HashSet<string> dependent = new HashSet<string>();
            dependent.Add(s);
            dependee_graph.Add(t, dependent);
        }
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
    }
}