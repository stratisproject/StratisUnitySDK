### List of tasks for server side

Fork full node and add endpoint to unityAPI feature that will receive information on user's progress. It should receive user's STRAX address which will act as identifier of the user, highest coins score, highest distance ran and it will return info on reward issued (no reward or txId). 

User gets CRS20 token rewards after getting 20\50\150\500 coins or after setting highest score of 20 / 40 / 70 / 100 / 200m.

Also user gets NFT once he runs the game for the first time. 



You need to deploy new SRC20 token and new NFT. Server's wallet should own all supply of SRC20 token and should be able to issue new NFTs. 



Server should store information about the user's progress and validate user's input (if user already received reward for getting 50 coins he can't receive that reward again). 



Addition endpoint with users statistics should be added- just show how many users played the game, how many users got how many rewards. 



Deploy this modified node on the server that Iain will provide and open port for unity API. 

