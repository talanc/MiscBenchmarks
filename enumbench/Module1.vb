Imports System.Runtime.CompilerServices
Imports BenchmarkDotNet.Attributes
Imports BenchmarkDotNet.Running

Public Module Module1
    Public Class EnumStrings(Of TEnum)
        Private Shared ReadOnly strings As String() = InitStrings()
        Private Shared Function InitStrings() As String()
            Dim values As Array = [Enum].GetValues(GetType(TEnum))
            Dim strings(0 To UBound(values)) As String
            For i As Integer = 0 To UBound(values)
                strings(i) = values(i).ToString()
            Next
            Return strings
        End Function

        Public Shared Function GetString(value As TEnum) As String
            Return strings(Convert.ToInt32(value))
        End Function
    End Class

    Public Enum TestEnum
        Field1
        Field2
    End Enum

    <Extension()>
    Public Function FastToString(value As TestEnum) As String
        Return EnumStrings(Of TestEnum).GetString(value)
    End Function

    Public Class EnumBenchmarks
        <Benchmark(Baseline:=True)>
        Public Function EnumToString() As String
            Return TestEnum.Field1.ToString()
        End Function

        <Benchmark()>
        Public Function FastToString() As String
            Return EnumStrings(Of TestEnum).GetString(TestEnum.Field1)
        End Function
    End Class

    Sub Main()
        Dim summary = BenchmarkRunner.Run(Of EnumBenchmarks)()
    End Sub

End Module
