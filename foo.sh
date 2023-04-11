if [[ $(git status --porcelain) ]]; then
    echo "It's dirty!"
else
    echo "It's clean"
fi
