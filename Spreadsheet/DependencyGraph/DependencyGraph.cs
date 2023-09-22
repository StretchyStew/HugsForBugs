// Skeleton implementation by: Joe Zachary, Daniel Kopta, Travis Martin for CS 3500
// Last updated: August 2023 (small tweak to API)

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
    //creates two dictionaries that will store respective dependent/dependee strings, and a HashSet
    private Dictionary<string, HashSet<string>> dependent_graph, dependee_graph;

    //Decided to count in the Add and Remove methods, so I needed this int.
    private int dependencyGraphSize;

    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        dependent_graph = new Dictionary<string, HashSet<string>>();
        dependee_graph = new Dictionary<string, HashSet<string>>();
        dependencyGraphSize = 0;
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
        if (dependent_graph.ContainsKey(s))
        {
            return dependent_graph[s].Count;
        }
        else
            return 0;
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        //if dependee contains the dependent s, then it will return true
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
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        //if dependent contains the dependee s, then it will return true
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
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        //first checks to see if 's' is in dependee_graph
        if (dependee_graph.ContainsKey(s))
        {
            //if it is, then it will return a HashSet with the dependees
            return new HashSet<string>(dependee_graph[s]);
        }
        return new HashSet<string>();
    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        //first checks to see if 's' is in dependent_graph
        if (dependent_graph.ContainsKey(s))
        {
            //if it is, then it will return a HashSet with the dependents
            return new HashSet<string>(dependent_graph[s]);
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
        //Adds one to the count if dependent graph doesn't have the key 's' and dependee graph doesn't contain key 't'.
        if (!(dependee_graph.ContainsKey(s) && dependent_graph.ContainsKey(t)))
        {
            dependencyGraphSize++;
        }
        //if 't' is already in dependee, then it will just add it to the existing HashSet.
        if (dependee_graph.ContainsKey(s))
        {
            dependee_graph[s].Add(t);
        }
        //if 't; isn't already in dependee, then it will create a new HashSet for the dependee and assign t to it.
        else
        {
            HashSet<string> dependent = new HashSet<string>();
            dependent.Add(t);
            dependee_graph.Add(s, dependent);
        }
        //if 's' is already in dependents, then it will just add it to the existing HashSet.
        if (dependent_graph.ContainsKey(t))
        {
            dependent_graph[t].Add(s);
        }
        //if 's' ins't in dependents, then this will create a new HashSet for the dependent and assign s to it.
        else
        {
            HashSet<string> dependee = new HashSet<string>();
            dependee.Add(s);
            dependent_graph.Add(t, dependee);
        }
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
        //if dependent and dependee graph contain s and t, then this will subtract 1 from the graph size.
        if (dependee_graph.ContainsKey(s) && dependent_graph.ContainsKey(t))
        {
            dependencyGraphSize--;
        }
        //if dependee graph contains 's', then it will remove t (dependents), once all the values are removed,
        //then it will finally remove s (dependee).
        if (dependee_graph.ContainsKey(s))
        {
            dependee_graph[s].Remove(t);
            if (dependee_graph[s].Count() == 0)
            {
                dependee_graph.Remove(s);
            }
        }
        //if the dependent graph contains 't', then it will remove s (dependee), once all the values are removed,
        //then it will finally remove t (dependent).
        if (dependent_graph.ContainsKey(t))
        {
            dependent_graph[t].Remove(s);
            if (dependent_graph[t].Count() == 0)
            {
                dependee_graph.Remove(t);
            }
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
        //creates a enumerable variable to be used in the for each containing the dependents of 's'
        IEnumerable<string> oldDependents = GetDependents(s);
        //for each dependent in old dependents, it will remove them
        foreach (string r in oldDependents)
        {
            RemoveDependency(s, r);
        }
        //for each new dependent in newDependents, it will make and add them to 't'
        foreach (string t in newDependents)
        {
            AddDependency(s, t);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
        //creates a enumerable variable to be used in the foreach containing the dependees of 's'
        IEnumerable<string> oldDependees = GetDependees(s);
        //for each dependee in oldDependees, it will remove them
        foreach (string r in oldDependees)
        {
            RemoveDependency(r, s);
        }
        //for each new dependee in newDependees, it will make and add them to 't'
        foreach (string t in newDependees)
        {
            AddDependency(t, s);
        }
    }
}