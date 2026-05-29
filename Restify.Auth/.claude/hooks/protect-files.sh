#!/bin/bash
# Hook: protect-files
# Prevents committing sensitive configuration files

PROTECTED_FILES=(
  "appsettings.Development.json"
)

for file in "${PROTECTED_FILES[@]}"; do
  if git diff --cached --name-only | grep -q "$file"; then
    echo "ERROR: No se permite commitear '$file'. Contiene secretos de desarrollo."
    echo "Usa 'git reset HEAD $file' para sacarlo del staging."
    exit 1
  fi
done

exit 0
