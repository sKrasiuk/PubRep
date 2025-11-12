package gologger

import (
	"fmt"
	"log"
	"os"
	"sync"
)

// Logger provides a structured, level-based logging interface that is safe for concurrent use.
type Logger struct {
	logger       *log.Logger
	logFile      *os.File
	minLevel     LogLevel
	mu           *sync.Mutex
	errorHandler func(error)
}

// LogLevel defines the severity of the log message.
type LogLevel int

// Log levels to control the verbosity of logs.
const (
	// LevelInfo is for informational messages.
	LevelInfo LogLevel = iota
	// LevelWarn is for warnings that might require attention.
	LevelWarn
	// LevelError is for errors that indicate a problem.
	LevelError
)

// NewFileLogger creates a logger that writes to a file.
// It is the caller's responsibility to call Close() on the returned logger, typically via defer.
//
//   - filename: The path to the log file. It will be created if it doesn't exist.
//   - minLevel: The minimum level of logs to write (e.g., LevelInfo, LevelWarn).
//   - flags: The log format flags from the standard log package (e.g., log.LstdFlags | log.Lshortfile).
//   - errorHandler: An optional function to handle errors during logging; if nil, a default handler is used.
func NewFileLogger(filename string, minLevel LogLevel, flags int, errorHandler func(error)) (*Logger, error) {
	file, err := os.OpenFile(filename, os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)
	if err != nil {
		return nil, err
	}
	l := log.New(file, "", flags)

	if errorHandler == nil {
		errorHandler = func(err error) { log.Printf("gologger write error: %v", err) }
	}
	return &Logger{logger: l, logFile: file, minLevel: minLevel, mu: &sync.Mutex{}, errorHandler: errorHandler}, nil
}

// NewConsoleLogger creates a logger that writes to the console (standard output).
//
//   - minLevel: The minimum level of logs to write (e.g., LevelInfo, LevelWarn).
//   - flags: The log format flags from the standard log package (e.g., log.LstdFlags | log.Lshortfile).
func NewConsoleLogger(minLevel LogLevel, flags int) *Logger {
	l := log.New(os.Stdout, "", flags)
	return &Logger{logger: l, logFile: nil, minLevel: minLevel, mu: &sync.Mutex{}, errorHandler: func(err error) {}}
}

// Close closes the log file if it was opened. It should be called, usually via defer,
// to ensure that all log entries are written to disk.
func (l *Logger) Close() {
	if l.logFile != nil {
		l.logFile.Close()
	}
}

// log is the internal logging method. It's concurrency-safe.
func (l *Logger) log(level LogLevel, message string) {
	if level < l.minLevel {
		return
	}

	prefix := ""
	switch level {
	case LevelInfo:
		prefix = "INFO: "
	case LevelWarn:
		prefix = "WARN: "
	case LevelError:
		prefix = "ERROR: "
	}

	l.mu.Lock()
	defer l.mu.Unlock()

	l.logger.SetPrefix(prefix)
	err := l.logger.Output(3, message)
	if err != nil {
		l.errorHandler(err)
	}
}

// Info logs a message at the Info level.
func (l *Logger) Info(message string) {
	l.log(LevelInfo, message)
}

// Warn logs a message at the Warn level.
func (l *Logger) Warn(message string) {
	l.log(LevelWarn, message)
}

// Error logs a message at the Error level.
func (l *Logger) Error(message string) {
	l.log(LevelError, message)
}

// Infof logs a formatted message at the Info level.
func (l *Logger) Infof(format string, v ...interface{}) {
	l.log(LevelInfo, fmt.Sprintf(format, v...))
}

// Warnf logs a formatted message at the Warn level.
func (l *Logger) Warnf(format string, v ...interface{}) {
	l.log(LevelWarn, fmt.Sprintf(format, v...))
}

// Errorf logs a formatted message at the Error level.
func (l *Logger) Errorf(format string, v ...interface{}) {
	l.log(LevelError, fmt.Sprintf(format, v...))
}
