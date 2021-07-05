package main

import "fmt"

func add(lhs byte, rhs byte) byte {
	return lhs ^ rhs
}

func multiply(lhs byte, rhs byte) byte {
	var resultPoly byte = 0
	var counter byte
	var hiBitSet byte
	for counter = 0; counter < 8; counter++ {
		if rhs&1 != 0 {
			resultPoly ^= lhs
		}
		hiBitSet = lhs & 0x80
		lhs <<= 1
		if hiBitSet != 0 {
			lhs ^= 0x1b
		}
		rhs >>= 1
	}
	return resultPoly
}

func multiplyMod(lhs uint32, rhs uint32, modulo uint16) uint64 {
	if lhs > rhs {
		lhs = lhs ^ rhs
		rhs = lhs ^ rhs
		lhs = lhs ^ rhs
	}
	var result uint64
	var tmpRhs uint64
	tmpRhs = uint64(rhs)
	for lhs > 0 {
		if lhs&1 != 0 {
			result = (result + tmpRhs) % uint64(modulo)
		}
		tmpRhs = (tmpRhs << 1) % uint64(modulo)
		lhs >>= 1
	}
	return result
}

func inverse(value uint8, modulo uint16) uint32 {
	var result uint32
	var pow uint16
	var tmp uint32
	result = 1
	pow = 254
	tmp = uint32(value)
	for pow > 0 {
		if pow&1 != 0 {
			result = uint32(multiplyMod(result, tmp, modulo))
		}
		tmp = uint32(multiplyMod(tmp, tmp, modulo))
		pow >>= 1
	}
	return result
}

func extentFunc(value uint16) uint16 {
	var number uint16
	var one uint16
	one = 1
	var counter uint8

	for ; counter < 13; counter++ {
		if one& value != 0{
			number = uint16(counter)
		}
		one <<= 1
	}
	return number
}

func divideMod(lhs uint16, rhs uint16) uint16 { /* lhs = lhs, rhs = rhs */
	var tmp uint16
	var firstExtent uint16
	var secondExtent uint16

	firstExtent = extentFunc(rhs)
	secondExtent = extentFunc(lhs)

	for firstExtent <= secondExtent {
		tmp = rhs << (secondExtent - firstExtent)
		lhs ^= tmp
		secondExtent = extentFunc(lhs)
	}
	return lhs
}

func findIrreduciblePolynomials() []uint16 {
	var items []uint16
	var isSimple bool
	for count := 257; count < 512; count += 2 {
		for secondCount := 3; secondCount < 32; secondCount++ {
			if divideMod(uint16(count), uint16(secondCount)) == 0 {
				isSimple = false
			}
		}
		if isSimple {
			items = append(items, uint16(count))
		}
		isSimple = true
	}
	return items
}

func main() {
	fmt.Println(add(5, 2))
	fmt.Println(multiply(5, 2))
	fmt.Println(inverse(25, 283))
	fmt.Println(findIrreduciblePolynomials())
}