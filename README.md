🛡️ AVcore - C# Security Suite
AVcore is a specialized security project born from a desire to bridge the gap between low-level network analysis and high-level file system protection. It is an evolving antivirus core that combines local file scanning with real-time cloud-based threat intelligence.

The Journey
After developing a Packet Sniffer, I wanted to push further into the cybersecurity domain. This project started with a simple question: "How does an antivirus actually work?" What seemed simple at first turned into an intensive deep dive. As of late 2025, I have invested over 11 hours into researching malware databases, debugging asynchronous file streams, and implementing security best practices. The project has shifted from a "simple script" idea into a structured .NET application utilizing modern async/await patterns.

Key Features
Advanced File Scanning (FileScanner)
The scanner doesn't just look at files; it understands the risks associated with them.

Zip-Bomb Protection: Monitors compression ratios and uncompressed sizes to prevent "decompression bombs" from crashing the system.

Zip-Slip Prevention: Validates extraction paths to ensure malicious archives cannot write files to sensitive system directories.

High-Performance Hashing (Hasher)
Asynchronous Processing: Uses SHA256.ComputeHashAsync to keep the UI/main thread responsive during heavy I/O.

Smart File Access: Implements FileShare.ReadWrite to allow scanning of files even if they are currently opened by other applications.

MalwareBazaar Integration
Real-time Intelligence: Integrates with the Abuse.ch MalwareBazaar API to verify file hashes against millions of known malware samples.

🛠️ Tech Stack
.NET 8

Concepts: Asynchronous Programming, Singleton Pattern,, API Integration.

Security: Cryptography (SHA256), Path Normalization, Archive Security.

⚠️ Disclaimer
This is an educational project currently in development. It is intended for research purposes and portfolio demonstration.
Also This project Is going through a full rewrite as of 22.3.2026 at [https://github.com/Tatu12335/AvCore] 
