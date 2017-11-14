# The idea YALC

1 log <=> src
2 (str, str, str) => Hash, Hash, Hash
3 int[] <=> int[]
4 Sorted look up map?
5.1 [sorted {hash, file hash, !symbol! position in file}] hash position in file - wont't work with regex "(.*)+?" (should be lazy) filtering(only position in file is known)
5.2 [{-1, 34326, 4}, {-1, 34326, 5}]

....."3 56 6567"
................
.."564 45 6"....

=> 

https://en.wikipedia.org/wiki/Longest_common_subsequence_problem