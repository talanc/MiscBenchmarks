#pragma once

#include "NativeMat.h"

#pragma managed

using namespace System;

namespace MmLibCpp
{
	public ref class CppMm
	{
	private:
		CppMm() { } // no constructor

	public:
		static void Managed_Mat12_Mat12(array<double, 2>^ result, array<double, 2>^ a, array<double, 2>^ b)
		{
			auto ar = a->GetLength(0), ac = a->GetLength(1);
			auto br = b->GetLength(0), bc = b->GetLength(1);

			if (ac != br)
			{
				throw gcnew InvalidOperationException("A.cols != B.rows");
			}

			//auto matrix = gcnew array<double, 2>(ar, bc);

			for (auto i = 1; i < ar; i++)
			{
				for (auto j = 1; j < bc; j++)
				{
					for (auto k = 1; k < ac; k++)
					{
						result[i, j] += a[i, k] * b[k, j];
					}
				}
			}
		}

		static void NativeWrapper_Mat12_Mat12(array<double, 2>^ result, array<double, 2>^ a, array<double, 2>^ b)
		{
			auto ar = a->GetLength(0), ac = a->GetLength(1);
			auto br = b->GetLength(0), bc = b->GetLength(1);

			if (ac != br)
			{
				throw gcnew InvalidOperationException("A.cols != B.rows");
			}

			if (ar != 13) {
				throw gcnew InvalidOperationException("Must be 13 rows/columns");
			}

			pin_ptr<double> pr = &result[0, 0];
			pin_ptr<double> pa = &a[0, 0];
			pin_ptr<double> pb = &b[0, 0];

			Native_Mat12_Mat12(pr, pa, pb);
		}
	};
}
