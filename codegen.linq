<Query Kind="VBStatements" />

For i As Integer = 1 To 12
    For j As Integer = 1 To 12
		Console.Write($"result({i},{j}) = 0 ")
        For k As Integer = 1 To 12
			Console.Write($"+ A({i}, {k}) * B({k}, {j})")
        Next
		Console.WriteLine()
    Next
Next
