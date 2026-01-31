🛡️ AVcore - C# Security Suite
AVcore is a specialized security project born from a desire to bridge the gap between low-level network analysis and high-level file system protection. It is an evolving antivirus core that combines local file scanning with real-time cloud-based threat intelligence.

🚀 The Journey
After developing a Packet Sniffer, I wanted to push further into the cybersecurity domain. This project started with a simple question: "How does an antivirus actually work?" What seemed simple at first turned into an intensive deep dive. As of late 2025, I have invested over 11 hours into researching malware databases, debugging asynchronous file streams, and implementing security best practices. The project has shifted from a "simple script" idea into a structured .NET application utilizing modern async/await patterns.

✨ Key Features
🔍 Advanced File Scanning (FileScanner)
The scanner doesn't just look at files; it understands the risks associated with them.

Zip-Bomb Protection: Monitors compression ratios and uncompressed sizes to prevent "decompression bombs" from crashing the system.

Zip-Slip Prevention: Validates extraction paths to ensure malicious archives cannot write files to sensitive system directories.

🧬 High-Performance Hashing (Hasher)
Asynchronous Processing: Uses SHA256.ComputeHashAsync to keep the UI/main thread responsive during heavy I/O.

Smart File Access: Implements FileShare.ReadWrite to allow scanning of files even if they are currently opened by other applications.

🌐 MalwareBazaar Integration
Real-time Intelligence: Integrates with the Abuse.ch MalwareBazaar API to verify file hashes against millions of known malware samples.

Professional Architecture: Uses a DTO (Data Transfer Object) pattern for clean JSON deserialization and a static HttpClient to prevent socket exhaustion.

🛠️ Tech Stack
Language: C# 12 / .NET 8

Concepts: Asynchronous Programming, Singleton Pattern, DTOs, API Integration.

Security: Cryptography (SHA256), Path Normalization, Archive Security.

📈 Roadmap & Future Plans
[ ] Magic Bytes Detection: Implement file signature headers (e.g., checking for MZ headers) to identify files even if their extensions are hidden.

[ ] Quarantine System: Safely isolate infected files by encrypting them or moving them to a restricted directory.

[ ] Unified Security Suite: Re-factor and integrate my previous Packet Sniffer to monitor network threats and file threats in one place.

[ ] Performance Optimization: Leveraging the upcoming hardware upgrades (new CPU cooler) to handle multi-threaded full-disk scans.

⚠️ Disclaimer
This is an educational project currently in development. It is intended for research purposes and portfolio demonstration.
