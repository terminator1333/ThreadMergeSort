using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace MultiThread2
{
    class Program
    {
        static void Main(string[] args)
        {

            MTMergeSort m = new MTMergeSort();

            string[] strList = GenerateRandomStrings(5000); // 5,000 random strings
            List<string> lst = new List<string>(m.MergeSort(strList));

            for (int i = 0; i < lst.Count; i++)
            {
                Console.WriteLine(lst[i]);
            }


            Console.WriteLine(strList.Length);
            Console.ReadLine();  // Keeps the console window open until you press Enter
        }
        public static string[] GenerateRandomStrings(int count, int minLength = 5, int maxLength = 10)
        {
            Random rng = new Random();
            string chars = "abcdefghijklmnopqrstuvwxyz";
            string[] result = new string[count];

            for (int i = 0; i < count; i++)
            {
                int length = rng.Next(minLength, maxLength + 1);
                char[] str = new char[length];

                for (int j = 0; j < length; j++)
                {
                    str[j] = chars[rng.Next(chars.Length)];
                }

                result[i] = new string(str);
            }

            return result;
        }
    }
    class MTMergeSort
    {


        public List<string> ThreadMerge(List<string> left, List<string> right)
        {
            List<string> result = new List<string>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (string.Compare(left[i], right[j]) <= 0)
                    result.Add(left[i++]);
                else
                    result.Add(right[j++]);
            }

            while (i < left.Count) result.Add(left[i++]);
            while (j < right.Count) result.Add(right[j++]);

            return result;
        }

        public void SortStringArray(string[] strList)
        {
            Array.Sort(strList, (x, y) => string.Compare(x, y));
        }

        public List<string> ThreadSortMerge(string[] strList, int start, int end, int nMin)
        {
            int length = end - start; //The legth of the part needed to be sorted
            if (length <= nMin) //incase the length is less that or equal to the minimum amount, just sort it
            {

                string[] sorted = strList.Skip(start).Take(length).ToArray(); //cutting the array to the relevat part. Take returns some other type so converting to an array
                SortStringArray(sorted); //sorting the array
                List<string> listSorted = sorted.ToList(); //creating new list and returning it
                return listSorted;
            }

            int mid = start + length / 2; //the middle index, where we will split the array to 2 subarrays

            List<string> leftList = null; //the left list
            List<string> rightList = null; //the right list

            Thread threadFirst = null; //the thread for the 1st half
            Thread threadSecond = null; //the thread for the 2d half

            if (mid - start <nMin) //if the amount of elements in the 1st half is less than nMin, sort it without opening a new thread
            {
                string[] sorted = strList.Skip(start).Take(length / 2).ToArray(); //cutting the array to the relevat part. Take returns some other type so converting to an array
                SortStringArray(sorted); //sorting the array
                leftList = sorted.ToList();

            }
            else
            {
                 threadFirst = new Thread(() => { leftList = ThreadSortMerge(strList, start, mid, nMin); }); //else, open the new thread with a recursive call to this fuction
            }

           if(end - mid < nMin) //like the first one, if the amount is too small to open a thread, just order it here
            {
                string[] sorted = strList.Skip(mid).Take(end - mid).ToArray(); //cutting the array to the relevat part. Take returns some other type so converting to an array
                SortStringArray(sorted); //sorting the array
                rightList = sorted.ToList(); //creating new list and returning it

            }

            else
            {
                 threadSecond = new Thread(() => { rightList = ThreadSortMerge(strList, mid, end, nMin); });
            }


           if (threadFirst != null) //if we created the first thread, then start it, same with the second thread
            {
                threadFirst.Start();
            }
            if (threadSecond != null)
            {
                threadSecond.Start();
            }
            if (threadFirst != null) //same here, if we created each thread then join it 
            {
                threadFirst.Join();
            }
            if (threadSecond != null)
            {
                threadSecond.Join();
            }
            return ThreadMerge(leftList, rightList); //finally, using the ThreadMerge fuction to merge the 2 sublists into one list
        }


        public List<string> MergeSort(string[] strList, int nMin = 50)
        {
            List<string> sortedList; //initialising the sorted list of strings

           sortedList =  ThreadSortMerge(strList,0,strList.Length, nMin); //applying the static sorting function

            return sortedList; //returning the list after sorting each string
        }
    }
}