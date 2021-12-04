package main

import (
	"fmt"
	"os"
	"strings"
)

func main() {
	var a = "message"
	fmt.Println(a)

	dat, err := os.ReadFile(os.Args[1])
	if err != nil {
		panic(err)
	}
	var filecontent = string(dat)
	var slices = strings.Split(filecontent, "\n")
	fmt.Print(slices)
}
