

if [ -z "$(git status --porcelain)" ]; then
    echo "It's clean"
else
    echo "It's dirty!"
fi
