package interfaces

type Saver interface {
	Save() error
}

type Printer interface {
	Print()
}

type Outputtable interface {
	Saver
	Printer
}