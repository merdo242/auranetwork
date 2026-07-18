<?php
// Aura Network Launcher API
// LimboAuth MySQL Authentication Bridge

header('Content-Type: application/json');

// --- DATABASE CONFIGURATION ---
// Please update these values with your actual LimboAuth database credentials!
$db_host = '127.0.0.1'; // Database IP (usually localhost/127.0.0.1)
$db_user = 'root';      // Database Username
$db_pass = '';          // Database Password (bos birakin)
$db_name = 'limboauth'; // Database Name

// LimboAuth Table Configuration
$table_name = 'limboauth'; // Tablo adi
$col_user = 'username';
$col_pass = 'password';

// --- HASHING ALGORITHM ---
// Allowed values: 'BCRYPT', 'ARGON2', 'SHA256'
$hash_algorithm = 'BCRYPT'; 
// -----------------------------

if (!isset($_GET['action'])) {
    die(json_encode(['error' => 'No action specified']));
}

$action = $_GET['action'];

try {
    $pdo = new PDO("mysql:host=$db_host;dbname=$db_name;charset=utf8mb4", $db_user, $db_pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die(json_encode(['error' => 'Database connection failed: ' . $e->getMessage()]));
}

if ($action === 'login') {
    if (!isset($_POST['username']) || !isset($_POST['password'])) {
        die(json_encode(['success' => false, 'error' => 'Missing username or password']));
    }

    $username = strtolower(trim($_POST['username']));
    $password = $_POST['password'];

    try {
        $stmt = $pdo->prepare("SELECT {$col_pass} FROM {$table_name} WHERE LOWER({$col_user}) = ? LIMIT 1");
        $stmt->execute([$username]);
        $row = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($row) {
            $hash = $row[$col_pass];
            $is_valid = false;

            if ($hash_algorithm === 'SHA256') {
                $is_valid = (hash('sha256', $password) === $hash);
            } else {
                $is_valid = password_verify($password, $hash);
            }

            if ($is_valid) {
                echo json_encode(['success' => true]);
            } else {
                echo json_encode(['success' => false, 'error' => 'Invalid password']);
            }
        } else {
            echo json_encode(['success' => false, 'error' => 'User not found']);
        }
    } catch (PDOException $e) {
        echo json_encode(['success' => false, 'error' => 'Query failed: ' . $e->getMessage()]);
    }
} 
elseif ($action === 'check') {
    if (!isset($_GET['username'])) {
        die(json_encode(['registered' => false]));
    }

    $username = strtolower(trim($_GET['username']));
    try {
        $stmt = $pdo->prepare("SELECT 1 FROM {$table_name} WHERE LOWER({$col_user}) = ? LIMIT 1");
        $stmt->execute([$username]);
        $exists = $stmt->fetchColumn() !== false;

        echo json_encode(['registered' => $exists]);
    } catch (PDOException $e) {
        echo json_encode(['registered' => false, 'error' => 'Query failed: ' . $e->getMessage()]);
    }
}
elseif ($action === 'register') {
    if (!isset($_POST['username']) || !isset($_POST['password'])) {
        die(json_encode(['success' => false, 'error' => 'Missing credentials']));
    }

    $username = strtolower(trim($_POST['username']));
    $password = $_POST['password'];

    try {
        $stmt = $pdo->prepare("SELECT 1 FROM {$table_name} WHERE LOWER({$col_user}) = ? LIMIT 1");
        $stmt->execute([$username]);
        if ($stmt->fetchColumn() !== false) {
            die(json_encode(['success' => false, 'error' => 'Username already taken']));
        }

        $hash = '';
        if ($hash_algorithm === 'SHA256') {
            $hash = hash('sha256', $password);
        } elseif ($hash_algorithm === 'ARGON2') {
            $hash = password_hash($password, PASSWORD_ARGON2I);
        } else {
            $hash = password_hash($password, PASSWORD_BCRYPT);
        }

        $stmt = $pdo->prepare("INSERT INTO {$table_name} ({$col_user}, {$col_pass}) VALUES (?, ?)");
        $success = $stmt->execute([$username, $hash]);

        echo json_encode(['success' => $success]);
    } catch (PDOException $e) {
        echo json_encode(['success' => false, 'error' => 'Query failed: ' . $e->getMessage()]);
    }
}
?>